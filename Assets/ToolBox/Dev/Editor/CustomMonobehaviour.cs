using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ToolBox.Dev; // pour ButtonAttribute

[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomMonobehaviour : Editor
{
    private Dictionary<string, List<SerializedProperty>> tabs = new();
    private List<string> tabOrder = new();
    private string currentTab;

    private void OnEnable()
    {
        tabs.Clear();
        tabOrder.Clear();

        SerializedProperty iterator = serializedObject.GetIterator();
        iterator.NextVisible(true); // skip "m_Script"

        string current = "DefaultTabName";

        while (iterator.NextVisible(false))
        {
            FieldInfo field = serializedObject.targetObject.GetType().GetField(
                iterator.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            // Check if this field defines a new tab
            TabAttribute tabAttr = field != null
                ? (TabAttribute)System.Attribute.GetCustomAttribute(field, typeof(TabAttribute))
                : null;

            if (tabAttr != null)
            {
                current = tabAttr.tabName;
                if (!tabs.ContainsKey(current))
                {
                    tabs[current] = new List<SerializedProperty>();
                    tabOrder.Add(current);
                }
            }

            // Always add the current field (including the one after [Tab])
            if (!tabs.ContainsKey(current))
            {
                tabs[current] = new List<SerializedProperty>();
                tabOrder.Add(current);
            }

            tabs[current].Add(serializedObject.FindProperty(iterator.name));
        }

        // 🧠 Sélectionne par défaut le premier onglet non "DefaultTabName"
        if (tabOrder.Count > 0)
        {
            if (tabOrder.Count > 1 && tabOrder[0] == "DefaultTabName")
                currentTab = tabOrder[1];
            else
                currentTab = tabOrder[0];
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (tabOrder.Count == 0 || (tabOrder.Count == 1 && tabOrder[0] == "DefaultTabName"))
        {
            base.OnInspectorGUI();
            DrawButtons();
            return;
        }

        SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
        if (scriptProp != null)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(scriptProp, true);
            GUI.enabled = true;
            EditorGUILayout.Space();
        }

        if (tabs.ContainsKey("DefaultTabName"))
        {
            foreach (var prop in tabs["DefaultTabName"])
                EditorGUILayout.PropertyField(prop, true);

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(2);
        }

        if (tabOrder.Count > 1 || (tabOrder.Count == 1 && tabOrder[0] != "DefaultTabName"))
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            foreach (var tab in tabOrder)
            {
                if (tab == "DefaultTabName") continue;

                bool selected = (tab == currentTab);
                if (GUILayout.Toggle(selected, tab, EditorStyles.toolbarButton))
                    currentTab = tab;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        if (tabs.ContainsKey(currentTab) && currentTab != "DefaultTabName")
        {
            foreach (var prop in tabs[currentTab])
                EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();

        DrawButtons();
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
                object[] parameters = ResolveParameters(buttonAttr.Parameters, target);
                method.Invoke(target, parameters);
            }
        }
    }

    private object[] ResolveParameters(object[] rawParams, object target)
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
}