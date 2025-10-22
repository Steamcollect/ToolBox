using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneBrowserPopUp : PopupWindowContent
{
    static int buttonHeight = 16;

    public override void OnGUI(Rect rect)
    {
        (string[] path, string[] name) scenes = GetAllScenesInAssetsRoot();

        for (int i = 0; i < scenes.path.Length; i++)
        {
            if (GUILayout.Button(scenes.name[i]))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenes.path[i]);
            }
        }
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, buttonHeight * GetSceneCountInProject());
    }

    public static int GetSceneCountInProject()
    {
        return AssetDatabase.FindAssets("t:Scene").Length;
    }

    public static (string[] paths, string[] names) GetAllScenesInAssetsRoot()
    {
        // Trouve toutes les scènes du projet
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

        // On utilise des listes dynamiques (plus simple à filtrer)
        var paths = new System.Collections.Generic.List<string>();
        var names = new System.Collections.Generic.List<string>();

        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Vérifie que la scène est dans le dossier "Assets/"
            // (FindAssets retourne parfois des assets du PackageCache ou autres)
            if (path.StartsWith("Assets/"))
            {
                paths.Add(path);
                names.Add(Path.GetFileNameWithoutExtension(path));
            }
        }

        return (paths.ToArray(), names.ToArray());
    }
}