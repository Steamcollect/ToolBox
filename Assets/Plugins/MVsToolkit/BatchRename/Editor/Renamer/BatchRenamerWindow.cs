using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public class BatchRenamerWindow : EditorWindow
    {
        private RenameConfig m_Config;
        private IRenamer m_Renamer;

        private List<IRenameTarget> m_Targets;
        private List<RenameResult> m_PreviewResults;


        [MenuItem("Tools/MVsToolkit/Batch Renamer")]
        private static void ShowWindow()
        {
            var window = GetWindow<BatchRenamerWindow>();
            window.Show();
        }

        private void Awake()
        {
            m_Config = new RenameConfig();
            m_Renamer = new RenamerService();
            m_Targets = new List<IRenameTarget>();
            m_PreviewResults = new List<RenameResult>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Batch Renamer", EditorStyles.boldLabel);

            if (GUILayout.Button("Preview Rename"))
            {
                PreviewRename();
            }

            if (m_PreviewResults.Count > 0)
            {
                GUILayout.Label("Preview Results:", EditorStyles.boldLabel);
                foreach (var result in m_PreviewResults)
                {
                    GUILayout.Label($"{result.OldName} -> {result.NewName}");
                }

                if (GUILayout.Button("Apply Rename"))
                {
                    ApplyRename();
                }
            }
        }

        private void PreviewRename() => m_PreviewResults = (List<RenameResult>)m_Renamer.Preview(m_Targets, m_Config);
        private void ApplyRename() => m_Renamer.Apply(m_PreviewResults, m_Config);

    }
}