using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

namespace ToolBox.BetterInterface
{
    public static class OpenLockedInspector
    {
        [MenuItem("GameObject/Open In New Inspector", false, 0)]
        [MenuItem("Assets/Open In New Inspector", false, 20)]
        public static void OpenLockedInspectorWindow()
        {
            UnityEngine.Object activeObject = Selection.activeObject;
            if (activeObject == null)
            {
                Debug.LogWarning("Aucun objet sélectionné.");
                return;
            }

            // Crée une nouvelle fenêtre d’inspector
            EditorWindow inspector = CreateNewInspectorWindow();

            // Lock la fenêtre sur l’objet sélectionné
            LockInspector(inspector, true);

            // Focus sur la nouvelle fenêtre
            inspector.Show();
            inspector.Focus();

            // Force la sélection de l’objet
            Selection.activeObject = activeObject;
        }

        private static EditorWindow CreateNewInspectorWindow()
        {
            // Utilise la même classe interne que l’inspector normal
            Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            return ScriptableObject.CreateInstance(inspectorType) as EditorWindow;
        }

        private static void LockInspector(EditorWindow inspector, bool isLocked)
        {
            // Récupère le champ interne "isLocked"
            Type inspectorType = inspector.GetType();
            PropertyInfo isLockedProp = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (isLockedProp != null)
            {
                isLockedProp.SetValue(inspector, isLocked, null);
            }

            // Force la mise à jour
            MethodInfo repaintMethod = inspectorType.GetMethod("Repaint", BindingFlags.Instance | BindingFlags.Public);
            repaintMethod?.Invoke(inspector, null);
        }
    }
}