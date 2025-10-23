using System.Collections.Generic;
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

    private const string SaveKey = "SceneBrowser_Data";

    Dictionary<string /*path*/, SceneBrowerData> scenesDataSaved = new();
    List<SceneBrowerData> favoriteScenes = new(), othersScenes = new();

    [System.Serializable]
    class SceneBrowerData
    {
        public string path;
        public string name;
        public bool isFavorite;

        public SceneBrowerData(string path, string name, bool isFavorite)
        {
            this.path = path;
            this.name = name;
            this.isFavorite = isFavorite;
        }
    }

    [System.Serializable]
    class SceneBrowerDataList
    {
        public List<SceneBrowerData> scenes = new List<SceneBrowerData>();
        public SceneBrowerDataList(IEnumerable<SceneBrowerData> data)
        {
            scenes.AddRange(data);
        }
    }

    GUIStyle leftButtonStyle;

    public override void OnOpen()
    {
        leftButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(10, 10, 2, 2),
        };

        LoadScenesData();
        
        (string[] paths, string[] names) = GetAllScenesInAssetsRoot();

        for (int i = 0; i < paths.Length; i++)
        {
            SceneBrowerData scene = new SceneBrowerData(
                    paths[i],
                    names[i],
                    scenesDataSaved.ContainsKey(paths[i]) ? scenesDataSaved[paths[i]].isFavorite : false
                );

            if(scene.isFavorite) favoriteScenes.Add(scene);
            else othersScenes.Add(scene);
        }
    }

    public override void OnClose()
    {
        scenesDataSaved.Clear();

        foreach (SceneBrowerData scene in favoriteScenes)
            scenesDataSaved.Add(scene.path, scene);
        foreach (SceneBrowerData scene in othersScenes)
            scenesDataSaved.Add(scene.path, scene);

        SaveScenesData();
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Space(2);
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        Event e = Event.current;

        // --- ZONE SCROLLABLE ---
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        if(favoriteScenes.Count > 0)
        {
            foreach (SceneBrowerData scene in favoriteScenes)
            {
                // Filtrage de la recherche
                if (!string.IsNullOrEmpty(searchQuery) &&
                    !scene.name.ToLower().Contains(searchQuery.ToLower()))
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(scene.name, leftButtonStyle, GUILayout.Height(buttonHeight)))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path);
                }

                GUIContent starIcon = new GUIContent(scene.isFavorite ? "★" : "☆");
                bool newFavorite = GUILayout.Toggle(scene.isFavorite, starIcon, "Button", GUILayout.Width(25));
                if (newFavorite != scene.isFavorite)
                {
                    scene.isFavorite = false;
                    favoriteScenes.Remove(scene);
                    othersScenes.Add(scene);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(5);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2) });
            GUILayout.Space(5);
        }

        foreach (var scene in othersScenes)
        {
            // Filtrage de la recherche
            if (!string.IsNullOrEmpty(searchQuery) &&
                !scene.name.ToLower().Contains(searchQuery.ToLower()))
            {
                continue;
            }

            var rowRect = GUILayoutUtility.GetRect(250, buttonHeight, GUILayout.ExpandWidth(true)); // Get space in GUILayout

            bool containsMouse = rowRect.Contains(Event.current.mousePosition);

            float buttonWidth = rowRect.width - (containsMouse ? 36 : 9);
            var buttonRect = new Rect(rowRect.x + 5, rowRect.y, buttonWidth, rowRect.height);

            if (GUI.Button(buttonRect, scene.name, leftButtonStyle))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path);
            }
            GUILayout.Space(2);

            if (containsMouse)
            {
                Rect toggleRect = new Rect(buttonRect.xMax + 2, rowRect.y, rowRect.width - buttonWidth - 11, rowRect.height);

                if(GUI.Button(toggleRect, "☆"))
                {
                    scene.isFavorite = true;
                    othersScenes.Remove(scene);
                    favoriteScenes.Add(scene);
                    break;
                }
            }
        }

        GUILayout.EndScrollView();
    }

    void SaveScenesData()
    {
        SceneBrowerDataList list = new SceneBrowerDataList(scenesDataSaved.Values);
        string json = JsonUtility.ToJson(list, true);
        EditorPrefs.SetString(SaveKey, json);
    }
    void LoadScenesData()
    {
        scenesDataSaved.Clear();

        string json = EditorPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            SceneBrowerDataList list = JsonUtility.FromJson<SceneBrowerDataList>(json);
            if (list?.scenes != null)
            {
                foreach (SceneBrowerData scene in list.scenes)
                    scenesDataSaved.Add(scene.path,scene);
            }
        }
    }

    public override Vector2 GetWindowSize()
    {
        // Calcule le nombre d'éléments visibles après filtrage
        int visibleCount = 0;
        int offset = 14;

        if (favoriteScenes.Count > 0) // Favorite scenes
        {
            foreach (SceneBrowerData scene in favoriteScenes)
            {
                if (string.IsNullOrEmpty(searchQuery) ||
                    scene.name.ToLower().Contains(searchQuery.ToLower()))
                {
                    visibleCount++;
                }
            }

            offset += 28;
        }

        foreach (SceneBrowerData scene in othersScenes) // Other scenes
        {
            if (string.IsNullOrEmpty(searchQuery) ||
                scene.name.ToLower().Contains(searchQuery.ToLower()))
            {
                visibleCount++;
            }
        }

        // Hauteur théorique : barre de recherche + boutons + marges
        float idealHeight = searchBarHeight + (visibleCount * buttonHeight + visibleCount * 2) + offset;

        // On limite la hauteur max
        float finalHeight = Mathf.Min(maxHeight, idealHeight);

        return new Vector2(finalHeight == maxHeight ? 275 : 250, finalHeight);
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