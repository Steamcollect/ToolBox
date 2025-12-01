using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class MVsToolkitMonobehaviour : Editor
    {
        private readonly Dictionary<string, Dictionary<string, List<SerializedProperty>>> tabs = new();
        private readonly List<string> tabOrder = new();
        private string currentTab;

        private readonly Dictionary<string, List<SerializedProperty>> globalFoldouts = new();
        private readonly List<string> globalFoldoutOrder = new();

        private readonly List<HandleData> handles = new();
        private class HandleData
        {
            public SerializedProperty property;
            public FieldInfo field;
            public HandleAttribute attribute;
        }


        private void OnEnable()
        {
            if (serializedObject == null)
                return;

            InitializeData();
            ScanProperties(serializedObject, target);

            if (tabOrder.Count > 0)
                currentTab = tabOrder[0];
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null)
                return;

            serializedObject.Update();

            DrawScriptReference(serializedObject);
            DrawGlobalFoldouts(serializedObject);
            DrawTabs(serializedObject, target);

            serializedObject.ApplyModifiedProperties();

            DrawButtons();
        }

        #region Scene GUI
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
                if (h.field.FieldType == typeof(Vector3))
                {
                    Vector3 localValue = (Vector3)h.field.GetValue(target);

                    // Convertit en monde si Local
                    Vector3 worldValue = h.attribute.HandleType == TransformLocationType.Local
                        ? go.transform.TransformPoint(localValue)
                        : localValue;

                    Vector3 newWorldValue = Handles.PositionHandle(worldValue, Quaternion.identity);

                    // Convertit inverse si Local
                    Vector3 newLocalValue = h.attribute.HandleType == TransformLocationType.Local
                        ? go.transform.InverseTransformPoint(newWorldValue)
                        : newWorldValue;

                    h.field.SetValue(target, newLocalValue);
                }
                else if (h.field.FieldType == typeof(Vector2))
                {
                    Vector2 localValue = (Vector2)h.field.GetValue(target);

                    Vector3 worldValue = h.attribute.HandleType == TransformLocationType.Local
                        ? go.transform.TransformPoint((Vector3)localValue)
                        : (Vector3)localValue;

                    Vector3 newWorldValue = Handles.PositionHandle(worldValue, Quaternion.identity);

                    Vector2 newLocalValue = h.attribute.HandleType == TransformLocationType.Local
                        ? (Vector2)go.transform.InverseTransformPoint(newWorldValue)
                        : (Vector2)newWorldValue;

                    h.field.SetValue(target, newLocalValue);
                }
            }
        }
        #endregion

        #region Initialization & Scanning
        private void InitializeData()
        {
            tabs.Clear();
            tabOrder.Clear();
            globalFoldouts.Clear();
            globalFoldoutOrder.Clear();
        }

        private void ScanProperties(SerializedObject so, Object targetObj)
        {
            SerializedProperty iterator = so.GetIterator();
            if (!iterator.NextVisible(true))
                return;

            string currentTabName = null;
            string currentFoldoutName = string.Empty;

            do
            {
                if (iterator.name == "m_Script")
                    continue;

                FieldInfo field = GetFieldRecursive(targetObj.GetType(), iterator.name);

                if (field == null)
                    continue;

                TabAttribute tabAttr = field.GetCustomAttribute<TabAttribute>();
                FoldoutAttribute foldoutAttr = field.GetCustomAttribute<FoldoutAttribute>();
                HandleAttribute handleAttr = field.GetCustomAttribute<HandleAttribute>();

                if (tabAttr != null)
                {
                    currentTabName = tabAttr.tabName;
                    currentFoldoutName = string.Empty;
                    if (!tabs.ContainsKey(currentTabName))
                    {
                        tabs[currentTabName] = new Dictionary<string, List<SerializedProperty>>();
                        tabOrder.Add(currentTabName);
                    }
                }

                if (foldoutAttr != null)
                {
                    currentFoldoutName = foldoutAttr.foldoutName;
                    if (currentTabName == null)
                    {
                        if (!globalFoldouts.ContainsKey(currentFoldoutName))
                        {
                            globalFoldouts[currentFoldoutName] = new List<SerializedProperty>();
                            globalFoldoutOrder.Add(currentFoldoutName);
                        }
                    }
                    else
                    {
                        if (!tabs[currentTabName].ContainsKey(currentFoldoutName))
                            tabs[currentTabName][currentFoldoutName] = new List<SerializedProperty>();
                    }
                }

                SerializedProperty prop = so.FindProperty(iterator.name);
                if (prop == null)
                    continue;

                if (handleAttr != null)
                {
                    handles.Add(new HandleData
                    {
                        property = prop,
                        field = field,
                        attribute = handleAttr
                    });
                }

                string foldoutKey = string.IsNullOrEmpty(currentFoldoutName) ? "_root_" : currentFoldoutName;
                if (currentTabName == null)
                {
                    if (!globalFoldouts.ContainsKey(foldoutKey))
                    {
                        globalFoldouts[foldoutKey] = new List<SerializedProperty>();
                        if (foldoutKey != "_root_")
                            globalFoldoutOrder.Add(foldoutKey);
                    }
                    globalFoldouts[foldoutKey].Add(prop);
                }
                else
                {
                    if (!tabs[currentTabName].ContainsKey(foldoutKey))
                        tabs[currentTabName][foldoutKey] = new List<SerializedProperty>();
                    tabs[currentTabName][foldoutKey].Add(prop);
                }

            } while (iterator.NextVisible(false));
        }
        #endregion

        #region Drawing
        private void DrawScriptReference(SerializedObject so)
        {
            SerializedProperty scriptProp = so.FindProperty("m_Script");
            if (scriptProp != null)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(scriptProp);
                GUI.enabled = true;
                EditorGUILayout.Space();
            }
        }

        private void DrawGlobalFoldouts(SerializedObject so)
        {
            if (globalFoldouts.ContainsKey("_root_"))
            {
                foreach (var prop in globalFoldouts["_root_"])
                    EditorGUILayout.PropertyField(prop, true);
            }

            foreach (string foldoutName in globalFoldoutOrder)
                DrawFoldout(foldoutName, globalFoldouts[foldoutName], "Global");

            if (globalFoldouts.Count > 0 && tabs.Count > 0)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.Space(4);
            }
        }

        private void DrawTabs(SerializedObject so, Object targetObj)
        {
            if (tabs.Count == 0)
                return;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            foreach (var tabName in tabOrder)
            {
                bool selected = (tabName == currentTab);
                if (GUILayout.Toggle(selected, tabName, EditorStyles.toolbarButton))
                    currentTab = tabName;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (tabs.TryGetValue(currentTab, out var groups))
            {
                foreach (var kvp in groups)
                {
                    string foldoutName = kvp.Key;
                    var props = kvp.Value;

                    if (foldoutName == "_root_")
                    {
                        foreach (SerializedProperty prop in props)
                            EditorGUILayout.PropertyField(prop, true);
                        EditorGUILayout.Space(4);
                        continue;
                    }

                    DrawFoldout(foldoutName, props, currentTab);
                }
            }
        }

        private void DrawFoldout(string foldoutName, List<SerializedProperty> props, string context)
        {
            string foldoutKey = $"{target.GetType().Name}_{context}_{foldoutName}_Foldout";
            bool isExpanded = EditorPrefs.GetBool(foldoutKey, true);
            bool newExpanded = EditorGUILayout.Foldout(isExpanded, foldoutName, true, EditorStyles.foldout);

            if (newExpanded != isExpanded)
                EditorPrefs.SetBool(foldoutKey, newExpanded);

            if (newExpanded)
            {
                EditorGUI.indentLevel++;
                foreach (var prop in props)
                    EditorGUILayout.PropertyField(prop, true);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }
        }

        private void DrawButtons()
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
        private FieldInfo GetFieldRecursive(System.Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(
                    fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                );
                if (field != null)
                    return field;

                type = type.BaseType;
            }
            return null;
        }

        #endregion
    }
}