using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using static UnityEngine.GUILayout;

namespace MVsToolkit.BatchRename
{
    public class BatchRenamerWindow : EditorWindow
    {
        #region Fields

        private SSO_RenamePreset m_Preset;
        private SSO_RenamePreset m_DefaultPreset;
        private RenameConfig m_Config;
        private IRenamer m_Renamer;

        private List<IRenameTarget> m_Targets;
        private List<RenameResult> m_PreviewResults;

        private Vector2 m_TargetListScroll;
        private Vector2 m_ConfigScroll;

        private SerializedObject m_PresetSerializedObject;
        private SerializedObject m_DefaultPresetSerializedObject;
        private SerializedProperty m_PresetConfigProp;

        private Object[] m_SelectionSnapshot;

        private static string s_PathLastUsedPreset;
        private static readonly string s_BatchRenamerTitle = "Batch Renamer";
        private static readonly string s_PropertyPresetName = "Config";
        private const int k_PriorityMenuItem = 0;

        #endregion

        #region Menu Items

        [MenuItem("GameObject/MVsToolkit/Batch Rename", false, k_PriorityMenuItem)]
        private static void ShowWindowFromGameObject()
        {
            ShowWindow();
        }

        [MenuItem("Assets/MVsToolkit/Batch Rename", false, k_PriorityMenuItem)]
        private static void ShowWindowFromAssets()
        {
            ShowWindow();
        }

        private static void ShowWindow()
        {
            BatchRenamerWindow window = GetWindow<BatchRenamerWindow>();
            window.Show();
        }

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            InitFields();

            InitializeTargetsFromSnapshot();

            InitializeDefaultPresetSerialize();

            if (!string.IsNullOrEmpty(s_PathLastUsedPreset)) LoadLastPreset();
        }

        private void LoadLastPreset()
        {
            SSO_RenamePreset lastPreset =
                AssetDatabase.LoadAssetAtPath<SSO_RenamePreset>(s_PathLastUsedPreset);
            if (lastPreset) InitializePresetSerialize(lastPreset);
        }

        private void OnDisable()
        {
            if (m_Preset) s_PathLastUsedPreset = AssetDatabase.GetAssetPath(m_Preset);
        }

        [InitializeOnLoadMethod]
        private static void InitRecompilation()
        {
            s_PathLastUsedPreset = null;
        }

        private void InitFields()
        {
            m_Renamer = new RenamerService();
            m_Targets = new List<IRenameTarget>();
            m_PreviewResults = new List<RenameResult>();
            m_ConfigScroll = Vector2.zero;

            m_SelectionSnapshot = new Object[Selection.objects.Length];
            Selection.objects.CopyTo(m_SelectionSnapshot, 0);

            titleContent = new GUIContent(s_BatchRenamerTitle);
        }

        #endregion

        #region Initialization

        private void InitializeTargetsFromSnapshot()
        {
            m_Targets.Clear();
            m_PreviewResults.Clear();

            if (m_SelectionSnapshot == null || m_SelectionSnapshot.Length == 0)
                return;

            foreach (Object obj in m_SelectionSnapshot)
            {
                if (obj == null)
                    continue;

                IRenameTarget target = obj is GameObject go 
                    ? new GameObjectTarget(go) 
                    : CreateAssetTarget(obj);

                if (target != null) 
                    m_Targets.Add(target);
            }
        }

        private IRenameTarget CreateAssetTarget(Object obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            return string.IsNullOrEmpty(assetPath) ? null : new AssetTarget(assetPath);
        }

        private void InitializePresetSerialize(SSO_RenamePreset newPreset)
        {
            if (newPreset == m_Preset) return;
            m_Preset = newPreset;

            if (!newPreset)
            {
                m_PresetConfigProp = null;
                m_PresetSerializedObject = null;
                m_Config = null;
                m_PreviewResults.Clear();
                return;
            }

            m_PresetSerializedObject = new SerializedObject(m_Preset);
            m_PresetConfigProp = m_PresetSerializedObject.FindProperty(s_PropertyPresetName);

            if (m_PresetSerializedObject == null || m_PresetConfigProp == null)
            {
                m_Config = null;
                return;
            }


            Debug.Log("Initialized preset: " + newPreset.name);
            m_Config = m_Preset.Config;

            PreviewRename();
        }

        private void InitializeDefaultPresetSerialize()
        {
            m_DefaultPreset = SSO_RenamePreset.DefaultPreset();
            m_DefaultPresetSerializedObject = new SerializedObject(m_DefaultPreset);
            m_PresetConfigProp = m_PresetSerializedObject?.FindProperty("Config");

            // SerializedObject or property must be null due to Instance lifetime, but work magic

            m_Config = m_DefaultPreset.Config;

            PreviewRename();
        }

        private void CreateNewPreset()
        {
            string savePath = EditorUtility.SaveFilePanelInProject(
                "Create New Rename Preset",
                "RenamePreset",
                "asset",
                "Save the preset asset"
            );

            if (string.IsNullOrEmpty(savePath)) return;

            SSO_RenamePreset newPreset = CreateInstance<SSO_RenamePreset>();
            AssetDatabase.CreateAsset(newPreset, savePath);
            AssetDatabase.SaveAssets();

            InitializePresetSerialize(newPreset);
        }

        #endregion

        #region Rendering

        private void OnGUI()
        {
            if (!hasFocus) return;

            Label(s_BatchRenamerTitle, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawLeftPane();

                DrawRightPane();
            }

            EditorGUILayout.Space();

            DrawButtonPanel();
        }

        private void DrawLeftPane()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, Width(position.width * 0.4f)))
            {
                DrawPresetSection();
                EditorGUILayout.Space();

                if (m_Preset)
                    DrawConfigPanel();
                else
                    DrawDefaultConfigFallback();

                FlexibleSpace();
            }
        }

        private void DrawRightPane()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, ExpandWidth(true)))
            {
                DrawTargetsPanel();
            }
        }

        private void DrawPresetSection()
        {
            EditorGUILayout.LabelField("Preset Asset", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                SSO_RenamePreset newPreset =
                    EditorGUILayout.ObjectField(m_Preset, typeof(SSO_RenamePreset), false) as SSO_RenamePreset;

                InitializePresetSerialize(newPreset);

                if (Button("Create New", Width(100))) CreateNewPreset();
            }
        }

        private void DrawConfigPanel()
        {
            if (m_PresetSerializedObject == null || m_PresetConfigProp == null)
            {
                EditorGUILayout.HelpBox("Config property not available.", MessageType.Warning);
                return;
            }

            DrawConfigSection(
                "Configuration Summary",
                false,
                m_PresetSerializedObject,
                m_PresetConfigProp,
                OnPresetConfigChanged);
        }

        private void DrawDefaultConfigFallback()
        {
            EditorGUILayout.HelpBox(
                "No preset selected. Showing default configuration for preview. " +
                "Create or assign a preset to save your configuration.",
                MessageType.Info);

            EditorGUILayout.Space();

            // Guard: null-check serialized fields
            if (m_DefaultPresetSerializedObject == null)
            {
                EditorGUILayout.HelpBox("Default config not available.", MessageType.Warning);
                return;
            }

            SerializedProperty configProp = m_DefaultPresetSerializedObject.FindProperty(s_PropertyPresetName);
            if (configProp == null)
            {
                EditorGUILayout.HelpBox("Config property not found in default preset.", MessageType.Warning);
                return;
            }

            DrawConfigSection(
                "Default Configuration",
                true,
                m_DefaultPresetSerializedObject,
                configProp,
                OnDefaultConfigChanged);
        }

        /// <summary>
        ///     Shared config rendering logic for both preset and default config.
        /// </summary>
        private void DrawConfigSection(
            string titleSection,
            bool isDefault,
            SerializedObject serializedObject,
            SerializedProperty configProperty,
            Action onChanged)
        {
            EditorGUILayout.LabelField(titleSection, EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                using (EditorGUILayout.ScrollViewScope scrollView = new(m_ConfigScroll, ExpandHeight(true)))
                {
                    m_ConfigScroll = scrollView.scrollPosition;

                    serializedObject.Update();

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(configProperty, new GUIContent(s_PropertyPresetName), true);

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        onChanged?.Invoke();
                    }

                    EditorGUILayout.Space();

                    if (!isDefault) return;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.HelpBox(
                            "This is a preview of the default configuration. " +
                            "Assign or create a preset to save your settings.",
                            MessageType.Info, true);

                        if (Button("Reset", Height(EditorGUIUtility.singleLineHeight * 2)))
                            InitializeDefaultPresetSerialize();
                    }
                }
            }
        }

        private void OnPresetConfigChanged()
        {
            if (m_Preset) m_Config = m_Preset.Config;

            PreviewRename();
        }

        private void OnDefaultConfigChanged()
        {
            m_Config = m_DefaultPreset.Config;
            PreviewRename();
        }

        private void DrawTargetsPanel()
        {
            EditorGUILayout.LabelField("Target List", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.largeLabel))
                {
                    EditorGUILayout.LabelField("Old Name", ExpandWidth(true));
                    EditorGUILayout.LabelField("New Name", ExpandWidth(true));
                }

                using (EditorGUILayout.ScrollViewScope scrollView = new(m_TargetListScroll, MaxHeight(400)))
                {
                    m_TargetListScroll = scrollView.scrollPosition;

                    if (m_Targets is { Count: 0 })
                        EditorGUILayout.HelpBox("No targets selected", MessageType.Info);
                    else
                        DrawTargetList();
                }
            }
        }

        private void DrawTargetList()
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                IRenameTarget target = m_Targets[i];
                RenameResult result = i < m_PreviewResults.Count ? m_PreviewResults[i] : null;

                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(target.Name, ExpandWidth(true));
                    DrawResultDisplay(result);
                }
            }
        }

        private void DrawResultDisplay(RenameResult result)
        {
            if (result == null)
            {
                EditorGUILayout.LabelField("(no preview)", EditorStyles.miniLabel, ExpandWidth(true));
                return;
            }

            if (result.HasError)
            {
                EditorGUILayout.LabelField(result.ErrorMessage, EditorStyles.miniLabel, ExpandWidth(true));
            }
            else if (result.HasConflict)
            {
                EditorGUILayout.LabelField(result.NewName, EditorStyles.miniLabel, ExpandWidth(true));
                EditorGUILayout.LabelField("(conflict)", EditorStyles.miniLabel, Width(60));
            }
            else
            {
                EditorGUILayout.LabelField(result.NewName, EditorStyles.miniLabel, ExpandWidth(true));
            }
        }

        private void DrawButtonPanel()
        {
            bool hasValidPreview = m_PreviewResults.Count > 0 && m_PreviewResults.TrueForAll(r => !r.HasError);
            bool canApply = hasValidPreview && m_Config != null;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (Button("Cancel", Height(30))) Close();

                EditorGUILayout.Space();

                using (new EditorGUI.DisabledScope(!canApply))
                {
                    if (Button("Apply Rename", Height(30))) ApplyRename();
                }
            }
        }

        #endregion

        #region IRenameService API Calls

        private void PreviewRename()
        {
            if (m_Targets.Count == 0)
            {
                EditorUtility.DisplayDialog("No targets", "Please select objects to rename.", "OK");
                return;
            }


            if (m_Config == null)
            {
                EditorUtility.DisplayDialog("No config", "No configuration available for preview.", "OK");
                return;
            }

            m_PreviewResults = (List<RenameResult>)m_Renamer.Preview(m_Targets, m_Config);
            Repaint();
        }

        private void ApplyRename()
        {
            if (m_PreviewResults.Count == 0)
            {
                EditorUtility.DisplayDialog("No preview", "Please generate a preview first.", "OK");
                return;
            }

            if (m_Config == null)
            {
                EditorUtility.DisplayDialog(
                    "Preset Required",
                    "You must select or create a preset before applying the rename. " +
                    "The default configuration is for preview only.",
                    "OK");
                return;
            }

            bool hasErrors = m_PreviewResults.Exists(r => r.HasError);
            bool hasConflicts = m_PreviewResults.Exists(r => r.HasConflict);

            if (hasErrors)
            {
                EditorUtility.DisplayDialog("Rename Failed", "Cannot apply rename: one or more results have errors.",
                    "OK");
                return;
            }

            if (hasConflicts)
            {
                int choice = EditorUtility.DisplayDialogComplex(
                    "Rename Conflicts",
                    "One or more rename targets have conflicts. Continue anyway?",
                    "Cancel",
                    "Continue",
                    ""
                );
                if (choice != 1)
                    return;
            }

            m_Renamer.Apply(m_PreviewResults, m_Config);

            m_PreviewResults.Clear();
            Repaint();

            Close();
        }

        #endregion
    }
}