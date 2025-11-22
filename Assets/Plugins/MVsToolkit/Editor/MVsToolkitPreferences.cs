using UnityEditor;

namespace MVsToolkit.BetterInterface
{
    public static class MVsToolkitPreferences
    {
        const string k_DrawFolderIconInHierarchyKey = "ToolBox_DrawFolderIconInHierarchy";
        public static bool s_DrawFolderIconInHierarchy
        {
            get => EditorPrefs.GetBool(k_DrawFolderIconInHierarchyKey, true);
            set => EditorPrefs.SetBool(k_DrawFolderIconInHierarchyKey, value);
        }

        //-----------------------------

        const string k_OverrideGameObjectIconKey = "ToolBox_OverrideGameObjectIcon";
        public static bool s_OverrideGameObjectIcon
        {
            get => EditorPrefs.GetBool(k_OverrideGameObjectIconKey, true);
            set => EditorPrefs.SetBool(k_OverrideGameObjectIconKey, value);
        }

        //-----------------------------

        const string k_ShowComponentsIconsKey = "k_ShowComponentsIcons";
        public static bool s_ShowComponentsIcon
        {
            get => EditorPrefs.GetBool(k_ShowComponentsIconsKey, true);
            set => EditorPrefs.SetBool(k_ShowComponentsIconsKey, value);
        }

        //-----------------------------

        const string k_IsZebraModeKey = "k_ZebraMode";
        public static bool s_IsZebraMode
        {
            get => EditorPrefs.GetBool(k_IsZebraModeKey, true);
            set => EditorPrefs.SetBool(k_IsZebraModeKey, value);
        }

        //==============================

        [SettingsProvider]
        public static SettingsProvider CreatePreferencesProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/ToolBox", SettingsScope.User)
            {
                label = "MV's Toolkit",

                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Hierarchy Settings", EditorStyles.boldLabel);
                    EditorGUILayout.Space();

                    s_DrawFolderIconInHierarchy = EditorGUILayout.Toggle("Draw Folder Icon", s_DrawFolderIconInHierarchy);
                    s_ShowComponentsIcon = EditorGUILayout.Toggle("Show Components Icon", s_ShowComponentsIcon);
                    s_OverrideGameObjectIcon = EditorGUILayout.Toggle("Override GameObject Icon", s_OverrideGameObjectIcon);

                    EditorGUILayout.Space();
                    s_IsZebraMode = EditorGUILayout.Toggle("Zebra Mode", s_IsZebraMode);
                },

                // recherche
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "toolbox", "hierarchy", "gameobject", "icon" })
            };

            return provider;
        }
    }
}