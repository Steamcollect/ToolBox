using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomHierarchyWindow : EditorWindow
{
    [MenuItem("Window/Custom Hierarchy")]
    public static void ShowWindow()
    {
        GetWindow<CustomHierarchyWindow>("Custom Hierarchy");
    }

    private Vector2 scroll;

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (var scene in GetAllScenes())
        {
            EditorGUILayout.LabelField($"Scene: {scene.name}", EditorStyles.boldLabel);
            foreach (var root in scene.GetRootGameObjects())
            {
                DrawGameObjectRecursive(root, 0);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawGameObjectRecursive(GameObject go, int indent)
    {
        EditorGUI.indentLevel = indent;
        EditorGUILayout.ObjectField(go.name, go, typeof(GameObject), true);
        foreach (Transform child in go.transform)
        {
            DrawGameObjectRecursive(child.gameObject, indent + 1);
        }
    }

    private static Scene[] GetAllScenes()
    {
        int count = SceneManager.sceneCount;
        Scene[] scenes = new Scene[count];
        for (int i = 0; i < count; i++)
            scenes[i] = SceneManager.GetSceneAt(i);
        return scenes;
    }
}