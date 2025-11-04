using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ToolBox.Dev;

[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomMonobehaviour : Editor
{
    private readonly Dictionary<string, Dictionary<string, List<SerializedProperty>>> tabs = new();
    private readonly List<string> tabOrder = new();
    private string currentTab;

    // Default groups
    private readonly Dictionary<string, List<SerializedProperty>> globalFoldouts = new();
    private readonly List<string> globalFoldoutOrder = new();

    private void OnEnable()
    {
        tabs.Clear();
        tabOrder.Clear();
        globalFoldouts.Clear();
        globalFoldoutOrder.Clear();

        SerializedProperty iterator = serializedObject.GetIterator();
        if (!iterator.NextVisible(true))
            return;

        string currentTabName = null;
        string currentFoldoutName = string.Empty;

        do
        {
            if (iterator.name == "m_Script")
                continue;

            FieldInfo field = serializedObject.targetObject.GetType().GetField(
                iterator.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (field == null)
                continue;

            TabAttribute tabAttr = field.GetCustomAttribute<TabAttribute>();
            FoldoutAttribute foldoutAttr = field.GetCustomAttribute<FoldoutAttribute>();

            // Si on détecte un nouvel onglet
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

            // Détection d’un nouveau foldout
            if (foldoutAttr != null)
            {
                currentFoldoutName = foldoutAttr.foldoutName;

                // Si aucun onglet actif : foldout global
                if (currentTabName == null)
                {
                    if (!globalFoldouts.ContainsKey(currentFoldoutName))
                    {
                        globalFoldouts[currentFoldoutName] = new List<SerializedProperty>();
                        globalFoldoutOrder.Add(currentFoldoutName);
                    }
                }
                else // foldout dans tab
                {
                    if (!tabs[currentTabName].ContainsKey(currentFoldoutName))
                        tabs[currentTabName][currentFoldoutName] = new List<SerializedProperty>();
                }
            }

            // Récupération de la propriété sérialisée
            var prop = serializedObject.FindProperty(iterator.name);
            if (prop == null)
                continue;

            // Classement selon le contexte
            if (currentTabName == null)
            {
                string foldoutKey = string.IsNullOrEmpty(currentFoldoutName) ? "_root_" : currentFoldoutName;
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
                string foldoutKey = string.IsNullOrEmpty(currentFoldoutName) ? "_root_" : currentFoldoutName;
                if (!tabs[currentTabName].ContainsKey(foldoutKey))
                    tabs[currentTabName][foldoutKey] = new List<SerializedProperty>();
                tabs[currentTabName][foldoutKey].Add(prop);
            }

        } while (iterator.NextVisible(false));

        if (tabOrder.Count > 0)
            currentTab = tabOrder[0];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Script reference
        SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
        if (scriptProp != null)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(scriptProp);
            GUI.enabled = true;
            EditorGUILayout.Space();
        }

        // --- SECTION : Contenu global avant les tabs ---
        if (globalFoldouts.ContainsKey("_root_"))
        {
            foreach (var prop in globalFoldouts["_root_"])
                EditorGUILayout.PropertyField(prop, true);
        }

        foreach (string foldoutName in globalFoldoutOrder)
        {
            DrawFoldout(foldoutName, globalFoldouts[foldoutName], "Global");
        }

        if (globalFoldouts.Count > 0 && tabs.Count > 0)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(4);
        }

        // --- SECTION : Tabs ---
        if (tabs.Count > 0)
        {
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
                foreach (KeyValuePair<string, List<SerializedProperty>> kvp in groups)
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

        serializedObject.ApplyModifiedProperties();
        DrawButtons();
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
                object[] parameters = ResolveButtonsParameters(buttonAttr.Parameters, target);
                method.Invoke(target, parameters);
            }
        }
    }

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
}