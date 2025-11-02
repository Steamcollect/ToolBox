using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
public class GlobalInspectorContext : Editor
{
    public override void OnInspectorGUI()
    {
        // Bouton par défaut
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Open Locked Inspector"))
        {
            OpenLockedInspector.OpenLockedInspectorWindow();
        }
    }
}
