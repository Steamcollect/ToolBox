using UnityEditor;

namespace ToolBox.BetterInterface
{
    public static class ToolBoxPreferences
    {
        const string k_DrawModeInHierarchyKey = "ToolBox_DrawModeInHierarchy";
        public static ToolBoxIconDrawModeInHierarchy s_DrawModeInHierarchy
        {
            get => (ToolBoxIconDrawModeInHierarchy)EditorPrefs.GetInt(k_DrawModeInHierarchyKey, (int)ToolBoxIconDrawModeInHierarchy.OverrideGameObjectIcon);
            set => EditorPrefs.SetInt(k_DrawModeInHierarchyKey, (int)value);
        }

        //------------------------------

        const string k_DrawFolderIconInHierarchyKey = "ToolBox_DrawFolderIconInHierarchy";
        public static bool s_DrawFolderIconInHierarchy
        {
            get => EditorPrefs.GetBool(k_DrawFolderIconInHierarchyKey, true);
            set => EditorPrefs.SetBool(k_DrawFolderIconInHierarchyKey, value);
        }

        //==============================

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
                    s_DrawFolderIconInHierarchy = EditorGUILayout.Toggle("Draw Folder Icon in Hierarchy", s_DrawFolderIconInHierarchy);
                },

                // recherche
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "toolbox", "hierarchy", "gameobject", "icon" })
            };

            return provider;
        }
    }
}