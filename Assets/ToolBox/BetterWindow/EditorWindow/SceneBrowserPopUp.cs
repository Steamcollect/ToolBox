using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneBrowserPopUp : PopupWindowContent
{
    static int buttonHeight = 20;
    static int searchBarHeight = 22;
    static int maxHeight = 300;

    private Vector2 scrollPos;
    private string searchQuery = "";
    private (string[] path, string[] name) scenes;

    public override void OnOpen()
    {
        scenes = GetAllScenesInAssetsRoot();
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Space(2);
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        GUILayout.Space(4);

        // --- ZONE SCROLLABLE ---
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < scenes.path.Length; i++)
        {
            // Filtrage de la recherche
            if (!string.IsNullOrEmpty(searchQuery) &&
                !scenes.name[i].ToLower().Contains(searchQuery.ToLower()))
            {
                continue;
            }

            if (GUILayout.Button(scenes.name[i], GUILayout.Height(buttonHeight)))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenes.path[i]);
            }
        }

        GUILayout.EndScrollView();
    }

    public override Vector2 GetWindowSize()
    {
        // Calcule le nombre d'éléments visibles après filtrage
        int visibleCount = 0;
        foreach (var name in scenes.name)
        {
            if (string.IsNullOrEmpty(searchQuery) ||
                name.ToLower().Contains(searchQuery.ToLower()))
            {
                visibleCount++;
            }
        }

        // Hauteur théorique : barre de recherche + boutons + marges
        float idealHeight = searchBarHeight + (visibleCount * buttonHeight + visibleCount * 2) + 12;

        // On limite la hauteur max
        float finalHeight = Mathf.Min(maxHeight, idealHeight);

        return new Vector2(250, finalHeight);
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