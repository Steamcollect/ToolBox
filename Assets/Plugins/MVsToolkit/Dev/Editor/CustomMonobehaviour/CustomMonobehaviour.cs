using System.Collections.Generic;
using System.Reflection;
using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class CustomMonobehaviour : Editor
{
    #region Fields
    public List<PropertyGroup> propertyGroups = new List<PropertyGroup>();
    #endregion

    #region Unity Callbacks
    private void OnEnable()
    {
        if (serializedObject == null)
            return;

        InitializeData();
        ScanProperties(serializedObject, target);
    }
    #endregion

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

                    EditorGUILayout.Space(4);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.Space(4);
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

            EditorGUILayout.Space(4);

            // Draw selected tab content
            var selectedTab = group.tabs[group.selectedTabIndex];
            if (selectedTab != null)
                DrawTab(selectedTab);

            EditorGUILayout.Space(6);

            // Optional separator between groups
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(4);
        }
    }

    private void DrawTab(TabGroup tab)
    {
        if (tab == null || tab.items == null) return;

        foreach (var item in tab.items)
        {
            DrawPropertyItem(item);
        }
    }

    private void DrawPropertyItem(PropertyItem item)
    {
        if (item == null) return;

        switch (item)
        {
            case PropertyField pf:
                if (pf.property != null)
                    EditorGUILayout.PropertyField(pf.property, true);
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
        if (fg == null) return;

        string foldoutKey = $"{target.GetType().Name}_Custom_{fg.Name}_Foldout";
        bool isExpanded = EditorPrefs.GetBool(foldoutKey, true);
        bool newExpanded = EditorGUILayout.Foldout(isExpanded, fg.Name, true, EditorStyles.foldout);

        if (newExpanded != isExpanded)
            EditorPrefs.SetBool(foldoutKey, newExpanded);

        if (newExpanded)
        {
            EditorGUI.indentLevel++;
            foreach (var field in fg.fields)
            {
                if (field?.property != null)
                    EditorGUILayout.PropertyField(field.property, true);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }
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