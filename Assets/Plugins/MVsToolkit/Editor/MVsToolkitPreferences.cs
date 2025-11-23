using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BetterInterface
{
    public static class MVsToolkitPreferences
    {
        #region Hierarchy Settings

        #region Boolean
        const string k_DrawFolderIconInHierarchyKey = "ToolBox_DrawFolderIconInHierarchyKey";
        public static bool s_DrawFolderIconInHierarchy
        {
            get => EditorPrefs.GetBool(k_DrawFolderIconInHierarchyKey, true);
            set => EditorPrefs.SetBool(k_DrawFolderIconInHierarchyKey, value);
        }

        //-----------------------------

        const string k_OverrideGameObjectIconKey = "ToolBox_OverrideGameObjectIconKey";
        public static bool s_OverrideGameObjectIcon
        {
            get => EditorPrefs.GetBool(k_OverrideGameObjectIconKey, true);
            set => EditorPrefs.SetBool(k_OverrideGameObjectIconKey, value);
        }

        //-----------------------------

        const string k_ShowComponentsIconsKey = "k_ShowComponentsIconsKey";
        public static bool s_ShowComponentsIcon
        {
            get => EditorPrefs.GetBool(k_ShowComponentsIconsKey, true);
            set => EditorPrefs.SetBool(k_ShowComponentsIconsKey, value);
        }

        //-----------------------------

        const string k_IsZebraModeKey = "k_ZebraModeKey";
        public static bool s_IsZebraMode
        {
            get => EditorPrefs.GetBool(k_IsZebraModeKey, true);
            set => EditorPrefs.SetBool(k_IsZebraModeKey, value);
        }

        //-----------------------------

        const string k_IsChildLineKey = "k_IsChildLineKey";
        public static bool s_IsChildLine
        {
            get => EditorPrefs.GetBool(k_IsChildLineKey, true);
            set => EditorPrefs.SetBool(k_IsChildLineKey, value);
        }
        #endregion


        #region Colors
        
        // ZEBRA
        
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

        // PREFAB
        
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

        // MISSING PREFAB

        const string k_EnableMissingPrefabColorKey = "k_EnableMissingPrefabColorKey";
        public static Color s_EnableMissingPrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_EnableMissingPrefabColorKey, "#FF6767"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_EnableMissingPrefabColorKey, colorString);
            }
        }

        //-----------------------------

        const string k_DisableMissingPrefabColorKey = "k_DisableMissingPrefabColorKey";
        public static Color s_DisableMissingPrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_DisableMissingPrefabColorKey, "#B64B4B"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_DisableMissingPrefabColorKey, colorString);
            }
        }

        //-----------------------------

        const string k_EnableSelectedMissingPrefabColorKey = "k_EnableSelectedMissingPrefabColorKey";
        public static Color s_EnableSelectedMissingPrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_EnableSelectedMissingPrefabColorKey, "#FFFFFF"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_EnableSelectedMissingPrefabColorKey, colorString);
            }
        }

        //-----------------------------

        const string k_DisableSelectedMissingPrefabColorKey = "k_DisableSelectedMissingPrefabColorKey";
        public static Color s_DisableSelectedMissingPrefabColor
        {
            get
            {
                string colorString = EditorPrefs.GetString(k_DisableSelectedMissingPrefabColorKey, "#E4AAAA"); // valeur par défaut
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                    return c;

                return Color.white;
            }
            set
            {
                string colorString = ColorUtility.ToHtmlStringRGBA(value);
                EditorPrefs.SetString(k_DisableSelectedMissingPrefabColorKey, colorString);
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
                    s_IsChildLine = EditorGUILayout.Toggle("Child Line", s_IsChildLine);
                    
                    EditorGUILayout.Space(30);

                    // Colors
                    s_ZebraSecondColor = EditorGUILayout.ColorField("Zebra Second Color", s_ZebraSecondColor);
                    EditorGUILayout.Space(10);
                    s_EnablePrefabColor = EditorGUILayout.ColorField("Enable Prefab Color", s_EnablePrefabColor);
                    s_DisablePrefabColor = EditorGUILayout.ColorField("Disable Prefab Color", s_DisablePrefabColor);
                    s_EnableSelectedPrefabColor = EditorGUILayout.ColorField("Enable Selected Prefab Color", s_EnableSelectedPrefabColor);
                    s_DisableSelectedPrefabColor = EditorGUILayout.ColorField("Disable Selected Prefab Color", s_DisableSelectedPrefabColor);

                    EditorGUILayout.Space(10);
                    s_EnableMissingPrefabColor = EditorGUILayout.ColorField("Enable Missing Prefab Color", s_EnableMissingPrefabColor);
                    s_DisableMissingPrefabColor = EditorGUILayout.ColorField("Disable Missing Prefab Color", s_DisableMissingPrefabColor);
                    s_EnableSelectedMissingPrefabColor = EditorGUILayout.ColorField("Enable Missing Selected Prefab Color", s_EnableSelectedMissingPrefabColor);
                    s_DisableSelectedMissingPrefabColor = EditorGUILayout.ColorField("Disable Missing Selected Prefab Color", s_DisableSelectedMissingPrefabColor);

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