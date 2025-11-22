using UnityEditor;

namespace MVsToolkit.BetterInterface
{
    public static class ToolBoxPreferences
    {
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

                    s_DrawFolderIconInHierarchy = EditorGUILayout.Toggle("Draw Folder Icon in Hierarchy", s_DrawFolderIconInHierarchy);
                },

                // recherche
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "toolbox", "hierarchy", "gameobject", "icon" })
            };

            return provider;
        }
    }
}