using System.Collections.Generic;
using System.Reflection;
using MVsToolkit.Utils;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class MVsCustomMonobehaviour : Editor
    {
        public List<MVsInspectorPropertyGroup> propertyGroups = new List<MVsInspectorPropertyGroup>();
        public List<MVsHandleData> handles = new List<MVsHandleData>();

        private readonly Dictionary<Color, Texture2D> _colorTextureCache = new();
        private readonly Color[] _tabPalette = new Color[0];

        // Cached help box style with zero top margin
        private GUIStyle _helpBoxNoTopMargin;

        // Cache for foldout expanded/collapsed states per FoldoutGroup instance
        private readonly Dictionary<MVsFoldoutGroup, bool> _foldoutStates = new();

        private void OnEnable()
        {
            if (serializedObject == null)
                return;

            InitializeData();
            ScanProperties(serializedObject, target);

            // Load saved tab and foldout states for this target
            LoadPersistentStates();
        }

        #region Initialization & Scanning
        void InitializeData()
        {
            propertyGroups.Clear();
            propertyGroups.Add(new MVsInspectorPropertyGroup(true));
            propertyGroups.GetLast().tabs.Add(new MVsTabGroup());
        }

        private void ScanProperties(SerializedObject so, Object targetObj)
        {
            if(so == null || targetObj == null)
                return;

            SerializedProperty iterator = so.GetIterator();
            if (!iterator.NextVisible(true))
                return;

            do
            {
                SerializedProperty prop = iterator.Copy();
                if (prop == null)
                    continue;

                string path = iterator.propertyPath ?? iterator.name;
                string rootName = path;

                int arrayIndex = path.IndexOf(".Array.data");
                if (arrayIndex >= 0)
                    rootName = path.Substring(0, arrayIndex);
                else
                {
                    int dot = path.IndexOf('.');
                    if (dot >= 0)
                        rootName = path.Substring(0, dot);
                }

                FieldInfo field = GetFieldRecursive(targetObj.GetType(), rootName, out bool isFirstField);
                if (field == null)
                    continue;

                if (TryGetCustomAttribute(field, out CloseTabAttribute closeTabAttr) || isFirstField) // Close Tab
                {
                    if (propertyGroups.Count > 0 && propertyGroups.GetLast().tabs.Count == 0)
                        propertyGroups.RemoveAt(propertyGroups.Count - 1);

                    propertyGroups.Add(new MVsInspectorPropertyGroup(true));
                }
                if (TryGetCustomAttribute(field, out TabAttribute tabAttr)) // Tab
                {
                    if (propertyGroups.Count == 0 || propertyGroups.GetLast().IsDrawByDefault)
                        propertyGroups.Add(new MVsInspectorPropertyGroup(false));

                    propertyGroups.GetLast().tabs.Add(new MVsTabGroup(tabAttr.tabName));
                }

                EnsureTabExists();

                if (TryGetCustomAttribute(field, out CloseFoldoutAttribute closeFoldoutAttr)) // Close Foldout
                {
                    if (propertyGroups.Count > 0 &&
                        propertyGroups.GetLast().tabs.Count > 0)
                    {
                        propertyGroups.GetLast().tabs.GetLast().currentFoldout = null;
                    }
                }
                if (TryGetCustomAttribute(field, out FoldoutAttribute foldoutAttr)) // Foldout
                {
                    MVsFoldoutGroup foldout = new MVsFoldoutGroup(foldoutAttr.foldoutName);
                    propertyGroups.GetLast().tabs.GetLast().items.Add(foldout);
                    propertyGroups.GetLast().tabs.GetLast().currentFoldout = foldout;
                }

                if (TryGetCustomAttribute(field, out HandleAttribute handleAttr)) // Handle
                {
                    handles.Add(new MVsHandleData
                    {
                        property = prop,
                        field = field,
                        attribute = handleAttr
                    });
                }

                if (propertyGroups.Count == 0)
                    InitializeData();

                if (propertyGroups.GetLast().tabs.Count == 0)
                    propertyGroups.GetLast().tabs.Add(new MVsTabGroup());

                if (propertyGroups.GetLast().tabs.GetLast().currentFoldout != null)
                    propertyGroups.GetLast().tabs.GetLast().currentFoldout.fields.Add(new MVsPropertyField(prop));
                else
                    propertyGroups.GetLast().tabs.GetLast().items.Add(new MVsPropertyField(prop));
            }
            while (iterator.NextVisible(false));
        }

        void EnsureTabExists()
        {
            if (propertyGroups.Count == 0)
                propertyGroups.Add(new MVsInspectorPropertyGroup(true));

            if (propertyGroups.GetLast().tabs.Count == 0)
                propertyGroups.GetLast().tabs.Add(new MVsTabGroup());
        }
        #endregion

        #region SceneDrawing
        private void OnSceneGUI()
        {
            DrawHandles();
        }

        void DrawHandles()
        {
            GameObject go = ((MonoBehaviour)target).gameObject;
            if (go != Selection.activeGameObject) return;

            foreach (var h in handles)
            {
                Vector3 localValue = Vector3.zero;
                Vector3 worldValue = Vector3.zero;

                if (h.field.FieldType == typeof(Vector3))
                    localValue = (Vector3)h.field.GetValue(target);
                else if (h.field.FieldType == typeof(Vector2))
                    localValue = (Vector2)h.field.GetValue(target);

                worldValue = h.attribute.spaceType == Space.Self
                    ? go.transform.TransformPoint(localValue)
                    : localValue;

                Handles.color = h.attribute.Color;

                Vector3 newWorldValue = worldValue;

                switch (h.attribute.DrawType)
                {
                    case HandleDrawType.Default:
                        newWorldValue = Handles.PositionHandle(worldValue, Quaternion.identity);

                        float size = HandleUtility.GetHandleSize(worldValue) * h.attribute.Size * .5f;
                        Handles.SphereHandleCap(
                            0,
                            worldValue,
                            Quaternion.identity,
                            size,
                            EventType.Repaint
                        ); break;

                    case HandleDrawType.Sphere:
                        newWorldValue = Handles.FreeMoveHandle(
                            worldValue,
                            HandleUtility.GetHandleSize(worldValue) * h.attribute.Size,
                            Vector3.zero,
                            Handles.SphereHandleCap);
                        break;

                    case HandleDrawType.Cube:
                        newWorldValue = Handles.FreeMoveHandle(
                            worldValue,
                            HandleUtility.GetHandleSize(worldValue) * h.attribute.Size,
                            Vector3.zero,
                            Handles.CubeHandleCap);
                        break;
                }

                Vector3 newLocalValue = h.attribute.spaceType == Space.Self
                    ? go.transform.InverseTransformPoint(newWorldValue)
                    : newWorldValue;

                Undo.RecordObject(target, "Handle Move");
                if (h.field.FieldType == typeof(Vector3))
                    h.field.SetValue(target, newLocalValue);
                else if (h.field.FieldType == typeof(Vector2))
                    h.field.SetValue(target, (Vector2)newLocalValue);

                EditorUtility.SetDirty(target);
            }
        }
        #endregion

        #region InspectorDrawing
        public override void OnInspectorGUI()
        {
            if (serializedObject == null)
                return;

            serializedObject.Update();

            DrawScriptField();

            DrawPropertyGroups();

            serializedObject.ApplyModifiedProperties();

            DrawButtons();
        }

        void DrawScriptField()
        {
            FieldInfo field = target.GetType().GetField(
                "m_Script",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            GUI.enabled = false;
            GUILayout.Space(1);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
            GUILayout.Space(3);
            GUI.enabled = true;
        }

        private void DrawPropertyGroups()
        {
            if (propertyGroups == null) return;

            for (int gIndex = 0; gIndex < propertyGroups.Count; gIndex++)
            {
                MVsInspectorPropertyGroup group = propertyGroups[gIndex];

                if (group == null || group.tabs == null || group.tabs.Count == 0) continue;

                if (group.IsDrawByDefault)
                {
                    foreach (MVsTabGroup tab in group.tabs)
                    {
                        if (tab != null)
                            DrawTab(tab);
                    }

                    continue;
                }

                string[] tabNames = new string[group.tabs.Count];
                for (int i = 0; i < group.tabs.Count; i++)
                {
                    string name = group.tabs[i]?.Name ?? "";
                    tabNames[i] = name == "MVsDefaultTab" ? $"Tab_{i + 1}" : name;
                }

                // Ensure selected index is within bounds
                if (group.selectedTabIndex < 0 || group.selectedTabIndex >= tabNames.Length)
                    group.selectedTabIndex = 0;

                // Wrap tabs to multiple rows depending on inspector width
                float inspectorWidth = EditorGUIUtility.currentViewWidth;
                float tabWidth = 100f; // estimated width per tab
                int tabsPerRow = Mathf.Max(1, Mathf.FloorToInt(inspectorWidth / tabWidth));

                int totalTabs = tabNames.Length;
                for (int start = 0; start < totalTabs; start += tabsPerRow)
                {
                    int end = Mathf.Min(start + tabsPerRow, totalTabs);
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    for (int j = start; j < end; j++)
                    {
                        bool selected = (j == group.selectedTabIndex);
                        if (GUILayout.Toggle(selected, tabNames[j], EditorStyles.toolbarButton))
                        {
                            if (group.selectedTabIndex != j)
                            {
                                group.selectedTabIndex = j;
                                SaveTabSelection(gIndex, j);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // Draw selected tab content
                var selectedTab = group.tabs[group.selectedTabIndex];
                if (selectedTab != null)
                {
                    // Use help box style with zero top margin so the draw box has top margin 0
                    EditorGUILayout.BeginVertical(GetHelpBoxStyle());
                    DrawTab(selectedTab);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space(6);
            }
        }

        private void DrawTab(MVsTabGroup tab)
        {
            if (tab == null || tab.items == null) return;

            GUILayout.Space(3);

            foreach (var item in tab.items)
            {
                DrawPropertyItem(item);
            }
            GUILayout.Space(3);
        }

        private void DrawPropertyItem(MVsPropertyItem item)
        {
            if (item == null) return;

            switch (item)
            {
                case MVsPropertyField pf:
                    if (pf.property != null)
                    {
                        if (pf.property.name == "m_Script")
                        {
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(pf.property, true);
                            GUI.enabled = true;
                            GUILayout.Space(4);
                        }
                        else EditorGUILayout.PropertyField(pf.property, true);
                    }
                    break;
                case MVsFoldoutGroup fg:
                    DrawFoldoutGroup(fg);
                    break;
                default:
                    break;
            }
        }

        void DrawFoldoutGroup(MVsFoldoutGroup fg)
        {
            if (fg == null) return;

            // Get or initialize foldout state
            if (!_foldoutStates.TryGetValue(fg, out bool expanded))
            {
                // Try to load persisted state for this foldout
                string foldNameKey = string.IsNullOrEmpty(fg.Name) ? "Foldout_" + fg.GetHashCode() : fg.Name;
                string key = GetPrefsPrefix() + "_foldout_" + foldNameKey;
                int saved = EditorPrefs.GetInt(key, -1);
                if (saved >= 0)
                    expanded = saved == 1;
                else
                    expanded = true; // default to expanded

                _foldoutStates[fg] = expanded;
            }

            EditorGUILayout.BeginVertical(GetHelpBoxStyle());

            // Foldout header
            EditorGUI.indentLevel++;
            bool newExpanded = EditorGUILayout.Foldout(expanded, string.IsNullOrEmpty(fg.Name) ? "" : fg.Name, true);
            EditorGUI.indentLevel--;

            // Save state
            if (newExpanded != expanded)
            {
                _foldoutStates[fg] = newExpanded;
                // Persist change
                string foldNameKey = string.IsNullOrEmpty(fg.Name) ? "Foldout_" + fg.GetHashCode() : fg.Name;
                string key = GetPrefsPrefix() + "_foldout_" + foldNameKey;
                EditorPrefs.SetInt(key, newExpanded ? 1 : 0);
            }

            // Draw fields when expanded
            if (newExpanded && fg.fields != null)
            {
                GUILayout.Space(2);
                foreach (var field in fg.fields)
                {
                    if (field != null)
                        DrawPropertyItem(field);
                }
                GUILayout.Space(2);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
        }

        void DrawButtons()
        {
            bool firstButton = true;
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttr == null)
                    continue;

                if (firstButton)
                {
                    firstButton = false;
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space(2);
                }

                if (GUILayout.Button(ObjectNames.NicifyVariableName(method.Name)))
                {
                    foreach (var t in targets)
                    {
                        object[] parameters = ResolveButtonsParameters(buttonAttr.Parameters, t);
                        method.Invoke(t, parameters);
                    }
                }
            }
        }
        #endregion

        #region Helpers
        private object[] ResolveButtonsParameters(object[] rawParams, object target)
        {
            if (rawParams == null)
                return null;

            object[] resolved = new object[rawParams.Length];
            for (int i = 0; i < rawParams.Length; i++)
            {
                object param = rawParams[i];
                if (param is string s)
                {
                    var field = target.GetType().GetField(s, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        resolved[i] = field.GetValue(target);
                        continue;
                    }
                }
                resolved[i] = param;
            }
            return resolved;
        }

        private FieldInfo GetFieldRecursive(System.Type type, string fieldName, out bool isFirtField)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(
                    fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                );
                if (field != null)
                {
                    if (type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0] == field)
                        isFirtField = true;
                    else
                        isFirtField = false;

                    return field;
                }
                type = type.BaseType;
            }
            isFirtField = false;
            return null;
        }

        bool TryGetCustomAttribute<T>(FieldInfo fieldInfo, out T attribute) where T : System.Attribute
        {
            attribute = fieldInfo.GetCustomAttribute<T>();
            if (attribute != null) return true;
            else return false;
        }

        private GUIStyle GetHelpBoxStyle()
        {
            if (_helpBoxNoTopMargin == null)
            {
                _helpBoxNoTopMargin = new GUIStyle(EditorStyles.helpBox);
                RectOffset m = _helpBoxNoTopMargin.margin;
                _helpBoxNoTopMargin.margin = new RectOffset(m.left, m.right, 0, m.bottom);
            }
            return _helpBoxNoTopMargin;
        }

        // Persistency helpers: store selections per target instance and component type
        private string GetPrefsPrefix()
        {
            if (target == null) return "MVsToolkit_unknown";
            return $"MVsToolkit_{target.GetInstanceID()}_{target.GetType().FullName}";
        }

        private void SaveTabSelection(int groupIndex, int selectedIndex)
        {
            string key = GetPrefsPrefix() + $"_group_{groupIndex}_selectedTab";
            EditorPrefs.SetInt(key, selectedIndex);
        }

        private void LoadPersistentStates()
        {
            // Load tabs
            for (int gIndex = 0; gIndex < propertyGroups.Count; gIndex++)
            {
                var group = propertyGroups[gIndex];
                string key = GetPrefsPrefix() + $"_group_{gIndex}_selectedTab";
                int saved = EditorPrefs.GetInt(key, group.selectedTabIndex);
                if (saved >= 0 && saved < group.tabs.Count)
                    group.selectedTabIndex = saved;
            }

            // Load foldouts
            for (int gIndex = 0; gIndex < propertyGroups.Count; gIndex++)
            {
                var group = propertyGroups[gIndex];
                for (int tIndex = 0; tIndex < group.tabs.Count; tIndex++)
                {
                    var tab = group.tabs[tIndex];
                    foreach (var item in tab.items)
                    {
                        if (item is MVsFoldoutGroup fg)
                        {
                            string foldNameKey = string.IsNullOrEmpty(fg.Name) ? "Foldout_" + fg.GetHashCode() : fg.Name;
                            string key = GetPrefsPrefix() + "_foldout_" + foldNameKey;
                            int saved = EditorPrefs.GetInt(key, -1);
                            bool expanded = saved >= 0 ? (saved == 1) : true;
                            _foldoutStates[fg] = expanded;
                        }
                    }
                }
            }
        }
        #endregion
    }
}