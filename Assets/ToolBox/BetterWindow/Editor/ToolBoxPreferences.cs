using UnityEditor;

namespace ToolBox.BetterWindow
{
    public static class ToolBoxPreferences
    {
        const string k_DrawModeInHierarchyKey = "ToolBox_DrawModeInHierarchy";

        public static ToolBoxIconDrawModeInHierarchy s_DrawModeInHierarchy
        {
            get => (ToolBoxIconDrawModeInHierarchy)EditorPrefs.GetInt(k_DrawModeInHierarchyKey, (int)ToolBoxIconDrawModeInHierarchy.OnGameObjectIcon);
            set => EditorPrefs.SetInt(k_DrawModeInHierarchyKey, (int)value);
        }

        [SettingsProvider]
        public static SettingsProvider CreatePreferencesProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/ToolBox", SettingsScope.User)
            {
                label = "Tool Box",

                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.LabelField("ToolBox Settings", EditorStyles.boldLabel);
                    EditorGUILayout.Space();

                    s_DrawModeInHierarchy = (ToolBoxIconDrawModeInHierarchy)EditorGUILayout.EnumPopup("Hierarchy Icons Draw Mode", s_DrawModeInHierarchy);
                },

                // recherche
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "toolbox", "hierarchy", "gameobject", "icon" })
            };

            return provider;
        }
    }
}