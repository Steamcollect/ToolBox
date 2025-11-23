using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BetterInterface
{
    public static class MVsToolkitPreferences
    {
        #region Hierarchy Settings

        #region Boolean
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
        #endregion

        //-----------------------------

        #region Colors
        const string k_ZebraSecondColor = "k_ZebraSecondColorKey";
        public static Color s_ZebraSecondColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_ZebraSecondColor, "#353535"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_ZebraSecondColor, colorString);
            }
        }

        //-----------------------------
        
        const string k_EnablePrefabColorKey = "k_EnablePrefabColorKey";
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

        const string k_DisablePrefabColorKey = "k_DisablePrefabColorKey";
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

        const string k_EnableSelectedPrefabColorKey = "k_EnableSelectedPrefabColorKey";
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

        const string k_DisableSelectedPrefabColorKey = "k_DisableSelectedPrefabColorKey";
        public static Color s_DisableSelectedPrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_DisableSelectedPrefabColorKey, "#99B1DC"); // valeur par défaut
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
        #endregion

        #endregion

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
                    
                    EditorGUILayout.Space(30);

                    // Colors
                    s_ZebraSecondColor = EditorGUILayout.ColorField("Zebra Second Color", s_ZebraSecondColor);
                    EditorGUILayout.Space(10);
                    s_EnablePrefabColor = EditorGUILayout.ColorField("Enable Prefab Color", s_EnablePrefabColor);
                    s_DisablePrefabColor = EditorGUILayout.ColorField("Disable Prefab Color", s_DisablePrefabColor);
                    EditorGUILayout.Space();
                    s_EnableSelectedPrefabColor = EditorGUILayout.ColorField("Enable Selected Prefab Color", s_EnableSelectedPrefabColor);
                    s_DisableSelectedPrefabColor = EditorGUILayout.ColorField("Disable Selected Prefab Color", s_DisableSelectedPrefabColor);

                    // Reset values button
                    EditorGUILayout.Space(20);
                    if (GUILayout.Button("Reset Values"))
                    {
                        bool confirm = EditorUtility.DisplayDialog(
                            "Reset Preferences",
                            "Are you sure you want to reset all MV's Toolkit preferences to their default values?",
                            "Yes",
                            "Cancel"
                        );

                        if (confirm)
                        {
                            // Reset booleans
                            s_DrawFolderIconInHierarchy = true;
                            s_OverrideGameObjectIcon = true;
                            s_ShowComponentsIcon = true;
                            s_IsZebraMode = true;

                            // Reset colors
                            s_ZebraSecondColor = ColorUtility.TryParseHtmlString("#353535", out var zc) ? zc : Color.white;
                            s_EnablePrefabColor = ColorUtility.TryParseHtmlString("#8CC7FF", out var epc) ? epc : Color.white;
                            s_DisablePrefabColor = ColorUtility.TryParseHtmlString("#6B90AE", out var dpc) ? dpc : Color.white;
                            s_EnableSelectedPrefabColor = Color.white;
                            s_DisableSelectedPrefabColor = ColorUtility.TryParseHtmlString("#99B1DC", out var dsc) ? dsc : Color.white;

                            Debug.Log("MV's Toolkit preferences reset to default values.");
                        }
                    }

                },

                // recherche
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "toolbox", "hierarchy", "gameobject", "icon" })
            };

            return provider;
        }
    }
}