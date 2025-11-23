using UnityEditor;
using UnityEngine;

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


        //-----------------------------

        const string k_EnablePrefabColorKey = "k_EnablePrefabColor";
        public static Color s_EnablePrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_EnablePrefabColorKey, "#8CC7FF"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_EnablePrefabColorKey, colorString);
            }
        }

        //-----------------------------

        const string k_DisablePrefabColorKey = "k_DisablePrefabColor";
        public static Color s_DisablePrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_DisablePrefabColorKey, "#6B90AE"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_DisablePrefabColorKey, colorString);
            }
        }

        //-----------------------------

        const string k_EnableSelectedPrefabColorKey = "k_EnableSelectedPrefabColor";
        public static Color s_EnableSelectedPrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_EnableSelectedPrefabColorKey, "#FFFFFF"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_EnableSelectedPrefabColorKey, colorString);
            }
        }

        //-----------------------------

        const string k_DisableSelectedPrefabColorKey = "k_DisableSelectedPrefabColor";
        public static Color s_DisableSelectedPrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_DisableSelectedPrefabColorKey, "#FFFFFF"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_DisableSelectedPrefabColorKey, colorString);
            }
        }

        //==============================

        [SettingsProvider]
        public static SettingsProvider CreatePreferencesProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/MVs Toolkit", SettingsScope.User)
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
                    
                    EditorGUILayout.Space(20);
                    s_EnablePrefabColor = EditorGUILayout.ColorField("Enable Prefab Color", s_EnablePrefabColor);
                    s_DisablePrefabColor = EditorGUILayout.ColorField("Disable Prefab Color", s_DisablePrefabColor);
                    EditorGUILayout.Space();
                    s_EnableSelectedPrefabColor = EditorGUILayout.ColorField("Enable Selected Prefab Color", s_EnableSelectedPrefabColor);
                    s_DisableSelectedPrefabColor = EditorGUILayout.ColorField("Disable Selected Prefab Color", s_DisableSelectedPrefabColor);
                },

                // recherche
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "toolbox", "hierarchy", "gameobject", "icon" })
            };

            return provider;
        }
    }
}