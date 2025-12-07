using System.Collections.Generic;
using System.Reflection;
using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class CustomMonobehaviour : Editor
{
    public List<PropertyGroup> propertyGroups = new List<PropertyGroup>();

    private readonly System.Collections.Generic.Dictionary<Color, Texture2D> _colorTextureCache = new();
    private readonly Color[] _tabPalette = new Color[0];

    private void OnEnable()
    {
        if (serializedObject == null)
            return;

        InitializeData();
        ScanProperties(serializedObject, target);
    }

    #region Initialization & Scanning
    void InitializeData()
    {
        propertyGroups.Clear();
        propertyGroups.Add(new PropertyGroup(true));
        propertyGroups.GetLast().tabs.Add(new TabGroup());
    }

    private void ScanProperties(SerializedObject so, Object targetObj)
    {
        SerializedProperty iterator = so.GetIterator();
        if (!iterator.NextVisible(true))
            return;

        do
        {
            // Determine root field name from propertyPath (handles nested properties and arrays)
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

            FieldInfo field = GetFieldRecursive(targetObj.GetType(), rootName);

            // Get attributes only if field found. We still want to include properties like m_Script
            TabAttribute tabAttr = field?.GetCustomAttribute<TabAttribute>();
            CloseTabAttribute closeTabAttr = field?.GetCustomAttribute<CloseTabAttribute>();

            if (closeTabAttr != null) // Close Tab
            {
                // Safe removal: ensure there's at least one group
                if (propertyGroups.Count > 0 && propertyGroups.GetLast().tabs.Count == 0)
                    propertyGroups.RemoveAt(propertyGroups.Count - 1);

                propertyGroups.Add(new PropertyGroup(true));
            }

            if (tabAttr != null) // Tab
            {
                if (propertyGroups.Count == 0 || propertyGroups.GetLast().IsDrawByDefault)
                    propertyGroups.Add(new PropertyGroup(false));

                propertyGroups.GetLast().tabs.Add(new TabGroup(tabAttr.tabName));
            }

            // Use a copy of the iterator so stored SerializedProperty isn't invalidated by iteration
            SerializedProperty prop = iterator.Copy();
            if (prop == null)
                continue;

            // Ensure we have at least one property group and one tab
            if (propertyGroups.Count == 0)
                InitializeData();

            if (propertyGroups.GetLast().tabs.Count == 0)
                propertyGroups.GetLast().tabs.Add(new TabGroup());

            propertyGroups.GetLast().tabs.GetLast().items.Add(new PropertyField(prop));
        }
        while (iterator.NextVisible(false));
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

    private Color GetTabColor(int index)
    {
        if (index < 0) index = 0;
        if (index >= _tabPalette.Length) index = _tabPalette.Length - 1;
        return _tabPalette[index];
    }

    private Texture2D GetColorTexture(Color color)
    {
        if (!_colorTextureCache.TryGetValue(color, out Texture2D texture) || texture == null)
        {
            texture = new Texture2D(1, 1)
            {
                hideFlags = HideFlags.DontSave,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat
            };
            texture.SetPixel(0, 0, color);
            texture.Apply();
            _colorTextureCache[color] = texture;
        }
        return texture;
    }

    private Color GetTabColor(PropertyGroup group, int tabIndex)
    {
        // kept for compatibility but now return neutral color
        return EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f, 1f) : new Color(0.93f, 0.93f, 0.93f, 1f);
    }
    #endregion

    #region Drawing
    public override void OnInspectorGUI()
    {
        if (serializedObject == null)
            return;

        serializedObject.Update();
        DrawPropertyGroups();

        serializedObject.ApplyModifiedProperties();

        DrawButtons();
    }

    private void DrawPropertyGroups()
    {
        if (propertyGroups == null) return;

        foreach (var group in propertyGroups)
        {
            if (group == null || group.tabs == null || group.tabs.Count == 0) continue;

            if (group.IsDrawByDefault)
            {
                // Draw all tabs contents directly, no tab bar
                foreach (var tab in group.tabs)
                {
                    // Optionally show tab name if not default
                    if (!string.IsNullOrEmpty(tab?.Name) && tab.Name != "MVsDefaultTab")
                    {
                        EditorGUILayout.LabelField(tab.Name, EditorStyles.boldLabel);
                    }

                    if (tab != null)
                        DrawTab(tab);
                }

                continue;
            }

            // Non-default behaviour: draw horizontal tabs and show selected only
            string[] tabNames = new string[group.tabs.Count];
            for (int i = 0; i < group.tabs.Count; i++)
            {
                string name = group.tabs[i]?.Name ?? "";
                tabNames[i] = name == "MVsDefaultTab" ? "" : name;
                if (string.IsNullOrEmpty(tabNames[i])) tabNames[i] = "Tab " + (i + 1);
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
                        group.selectedTabIndex = j;
                }
                EditorGUILayout.EndHorizontal();
            }

            // Draw selected tab content
            var selectedTab = group.tabs[group.selectedTabIndex];
            if (selectedTab != null)
            {
                // Use a helpBox vertical to ensure the content is visible with a subtle background
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawTab(selectedTab);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(6);
        }
    }

    private void DrawTab(TabGroup tab)
    {
        if (tab == null || tab.items == null) return;

        GUILayout.Space(3);

        foreach (var item in tab.items)
        {
            DrawPropertyItem(item);
        }
        GUILayout.Space(3);
    }

    private void DrawPropertyItem(PropertyItem item)
    {
        if (item == null) return;

        switch (item)
        {
            case PropertyField pf:
                if (pf.property != null)
                {
                    if(pf.property.name == "m_Script")
                    {
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(pf.property, true);
                        GUI.enabled = true;
                        GUILayout.Space(4);
                    }
                    else EditorGUILayout.PropertyField(pf.property, true);
                }
                break;
            case FoldoutGroup fg:
                DrawFoldoutGroup(fg);
                break;
            default:
                break;
        }
    }

    void DrawFoldoutGroup(FoldoutGroup fg)
    {

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
}