using System;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Utils
{
    public class BatchRenamerWindow : EditorWindow
    {
        [MenuItem("Tools/MVsToolkit/Batch Renamer")]
        public static void Init()
        {
            BatchRenamerWindow window = GetWindow<BatchRenamerWindow>("Batch Renamer");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Batch Renamer", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Select objects in the hierarchy and click the button below to rename them.",
                EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Rename Selected Objects"))
            {
                BatchRename();
            }
        }

        private void BatchRename()
        {
            var selectedObjects = Selection.objects;
            foreach (var obj in selectedObjects)
            {
                Debug.Log(obj.name);
            }
        }
        
    }
}