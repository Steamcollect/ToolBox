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
        private RenameConfig m_Config;
        private IRenamer m_Renamer;

        private List<IRenameTarget> m_Targets;
        private List<RenameResult> m_PreviewResults;

        private Vector2 m_TargetListScroll;
        private Vector2 m_ConfigScroll;
        
        private SerializedObject m_PresetSerializedObject;
        private SerializedProperty m_PresetConfigProp;
        
        private Object[] m_SelectionSnapshot;

        #endregion

        #region Menu Items

        [MenuItem("GameObject/MVsToolkit/Batch Rename", false, 20)]
        private static void ShowWindowFromGameObject()
        {
            ShowWindow();
        }

        [MenuItem("Assets/MVsToolkit/Batch Rename", false, 20)]
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
            m_Renamer = new RenamerService();
            m_Targets = new List<IRenameTarget>();
            m_PreviewResults = new List<RenameResult>();
            m_PresetSerializedObject = null;
            m_PresetConfigProp = null;
            m_Preset = null;
            m_Config = null;
            titleContent = new GUIContent("Batch Renamer");
            
            m_SelectionSnapshot = new Object[Selection.objects.Length];
            Selection.objects.CopyTo(m_SelectionSnapshot, 0);
            
            InitializeTargetsFromSnapshot();
            
            // Trigger initial preview if targets are available (config will be null until preset is set)
            if (m_Targets.Count > 0 && m_Config != null)
            {
                PreviewRename();
            }
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
                // Skip nullified objects from domain reloads
                if (obj == null)
                    continue;

                IRenameTarget target = null;
                
                if (obj is GameObject go)
                {
                    target = new GameObjectTarget(go);
                }
                else
                {
                    string assetPath = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        target = new AssetTarget(assetPath);
                    }
                }

                if (target != null)
                {
                    m_Targets.Add(target);
                }
            }
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
            m_PresetConfigProp = m_PresetSerializedObject?.FindProperty("Config");
            
            // Null-check: SerializedObject or Config property might be null if asset is invalid
            if (m_PresetSerializedObject == null || m_PresetConfigProp == null)
            {
                m_Config = null;
                return;
            }

            m_Config = m_Preset.Config;
            
            // Only preview if targets exist and config is valid
            if (m_Targets.Count > 0 && m_Config != null)
            {
                PreviewRename();
            }
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

            var newPreset = CreateInstance<SSO_RenamePreset>();
            AssetDatabase.CreateAsset(newPreset, savePath);
            AssetDatabase.SaveAssets();

            InitializePresetSerialize(newPreset);
        }

        #endregion
        
        #region Rendering

        private void OnGUI()
        {
            if (!hasFocus) return;
            
            Label("Batch Renamer", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Horizontal split: left pane (config) and right pane (targets)
            using (new EditorGUILayout.HorizontalScope())
            {
                // Left Pane: Preset and Config
                DrawLeftPane();
                
                // Right Pane: Targets
                DrawRightPane();
            }

            EditorGUILayout.Space();

            // Apply/Cancel Buttons at Bottom
            DrawButtonPanel();
        }

        private void DrawLeftPane()
        {
            using (new EditorGUILayout.VerticalScope( EditorStyles.helpBox ,Width(position.width*0.4f)))
            {
                // Preset Asset Section
                DrawPresetSection();
                EditorGUILayout.Space();

                // Config Panel Section
                if (m_Config != null)
                {
                    DrawConfigPanel();
                }
                else
                {
                    EditorGUILayout.HelpBox("No preset selected. Create or assign one.", MessageType.Info);
                }

                // Flexible space to push content up
                FlexibleSpace();
            }
        }

        private void DrawRightPane()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox,ExpandWidth(true)))
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

                if (Button("Create New", Width(100)))
                {
                    CreateNewPreset();
                }
            }
        }

        private void DrawConfigPanel()
        {
            // Guard: null-check serialized fields
            if (m_PresetSerializedObject == null || m_PresetConfigProp == null)
            {
                EditorGUILayout.HelpBox("Config property not available.", MessageType.Warning);
                return;
            }

            // Draw the preset's serialized Config property directly so the SSO is displayed "as-is".
            EditorGUILayout.LabelField("Configuration Summary", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(m_ConfigScroll, MaxHeight(300)))
                {
                    m_ConfigScroll = scrollView.scrollPosition;
                    
                    m_PresetSerializedObject.Update();

                    // Detect config changes with BeginChangeCheck/EndChangeCheck
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_PresetConfigProp,
                        new GUIContent("Config"),
                        true);
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_PresetSerializedObject.ApplyModifiedProperties();
                        
                        // Refresh config reference and trigger preview
                        if (m_Preset != null)
                        {
                            m_Config = m_Preset.Config;
                        }
                        
                        PreviewRename();
                    }
                }
            }
        }

        private void DrawTargetsPanel()
        {
            EditorGUILayout.LabelField("Target List", EditorStyles.boldLabel);
            
            using (new EditorGUI.IndentLevelScope())
            {
                // Header with Old and New columns
                using (new EditorGUILayout.HorizontalScope(EditorStyles.largeLabel))
                {
                    EditorGUILayout.LabelField("Old Name", ExpandWidth(true));
                    EditorGUILayout.LabelField("New Name", ExpandWidth(true));
                }
                
                using (var scrollView =
                       new EditorGUILayout.ScrollViewScope(m_TargetListScroll, MaxHeight(400)))
                {
                    m_TargetListScroll = scrollView.scrollPosition;

                    if (m_Targets is { Count: 0 })
                    {
                        EditorGUILayout.HelpBox("No targets selected", MessageType.Info);
                    }
                    else
                    {
                        DrawTargetList();
                    }
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
                    // Old Name (left side)
                    EditorGUILayout.LabelField(target.Name, ExpandWidth(true));
                    
                    // New Name (right side) with status
                    if (result != null)
                    {
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
                    else
                    {
                        EditorGUILayout.LabelField("(no preview)", EditorStyles.miniLabel, ExpandWidth(true));
                    }
                }
            }
        }

        private void DrawButtonPanel()
        {
            bool hasValidPreview = m_PreviewResults.Count > 0 && m_PreviewResults.TrueForAll(r => !r.HasError);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                // Cancel button (always enabled)
                if (Button("Cancel", Height(30)))
                {
                    Close();
                }

                EditorGUILayout.Space();

                // Apply button (disabled if no valid preview or errors exist)
                using (new EditorGUI.DisabledScope(!hasValidPreview))
                {
                    if (Button("Apply Rename", Height(30)))
                    {
                        ApplyRename();
                    }
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
                EditorUtility.DisplayDialog("No config", "Please select or create a preset.", "OK");
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

            // Short-circuit if any errors or conflicts exist
            bool hasErrors = m_PreviewResults.Exists(r => r.HasError);
            bool hasConflicts = m_PreviewResults.Exists(r => r.HasConflict);

            if (hasErrors)
            {
                EditorUtility.DisplayDialog("Rename Failed", "Cannot apply rename: one or more results have errors.", "OK");
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
                if (choice != 1) // User did not click "Continue"
                    return;
            }

            m_Renamer.Apply(m_PreviewResults, m_Config);

            m_PreviewResults.Clear();
            Repaint();
            
            // Auto-close window after successful apply
            Close();
        }

        #endregion
    }
}