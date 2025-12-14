using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneBrowserContent
{
    static int buttonHeight = 20;
    static int searchBarHeight = 22;

    static Vector2 scrollPos;
    static string searchQuery = "";

    static Dictionary<string /*path*/, SceneBrowerData> scenesDataSaved = new();
    const string SaveKey = "SceneBrowser_Data";

    static List<SceneBrowerData> favoriteScenes = new(), othersScenes = new();

    static Stack<System.Action> undoStack = new();

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
        public List<SceneBrowerData> scenes;
        public SceneBrowerDataList(IEnumerable<SceneBrowerData> data)
        {
            // initialize from enumerable without duplicating entries
            scenes = new List<SceneBrowerData>(data);
        }
    }

    static GUIStyle leftButtonStyle = new GUIStyle(GUI.skin.button)
    {
        alignment = TextAnchor.MiddleLeft,
        padding = new RectOffset(10, 10, 2, 2),
    };
    static GUIStyle RightTextStyle = new GUIStyle(EditorStyles.label)
    {
        alignment = TextAnchor.MiddleRight,
        fontStyle = FontStyle.Bold,
        padding = new RectOffset(10, 10, 2, 2),
    };

    public static void OnOpen()
    {
        LoadScenesData();

        (string[] paths, string[] names) = GetAllScenesInAssetsRoot();

        for (int i = 0; i < paths.Length; i++)
        {
            SceneBrowerData scene = new SceneBrowerData(
                    paths[i],
                    names[i],
                    scenesDataSaved.ContainsKey(paths[i]) ? scenesDataSaved[paths[i]].isFavorite : false
                );

            if (scene.isFavorite) favoriteScenes.Add(scene);
            else othersScenes.Add(scene);
        }
    }
    public static void OnClose()
    {
        scenesDataSaved.Clear();

        foreach (SceneBrowerData scene in favoriteScenes)
            scenesDataSaved.Add(scene.path, scene);
        foreach (SceneBrowerData scene in othersScenes)
            scenesDataSaved.Add(scene.path, scene);

        SaveScenesData();
    }

    public static void OnGUI(Rect rect, string searchTxt)
    {
        searchQuery = searchTxt;

        GUILayout.Space(10);

        Event e = Event.current;
        DrawButtons(e);
        CheckUndo(e);
    }

    static void DrawButtons(Event e)
    {
        // --- ZONE SCROLLABLE ---
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        if (favoriteScenes.Count > 0)
        {
            // iterate over snapshot to avoid modifying during enumeration
            foreach (SceneBrowerData scene in favoriteScenes.ToArray())
                DrawSceneButton(scene);
        }


        if (favoriteScenes.Count > 0 && othersScenes.Count > 0)
        {
            GUILayout.Space(5);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2) });
            GUILayout.Space(5);
        }

        foreach (var scene in othersScenes.ToArray())
            DrawSceneButton(scene);

        GUILayout.EndScrollView();
    }

    static void DrawSceneButton(SceneBrowerData scene)
    {
        if (!string.IsNullOrEmpty(searchQuery) &&
                !scene.name.ToLower().Contains(searchQuery.ToLower()))
        {
            return;
        }

        var rowRect = GUILayoutUtility.GetRect(250, buttonHeight, GUILayout.ExpandWidth(true)); // Get space in GUILayout

        bool containsMouse = rowRect.Contains(Event.current.mousePosition);

        float buttonWidth = rowRect.width - (containsMouse ? 36 : 9);
        var buttonRect = new Rect(rowRect.x + 5, rowRect.y, buttonWidth, rowRect.height);

        if (GUI.Button(buttonRect, scene.name, leftButtonStyle))
        {
            // Vérifie si la scène actuelle est modifiée
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                int option = EditorUtility.DisplayDialogComplex(
                    "Scene Modified",
                    "The scene as been modified. " +
                    "Would you want to save it?", "Yes",
                    "No",
                    "Cancel"
                );

                if (option == 0)
                {
                    // Sauvegarder et changer de scène
                    EditorSceneManager.SaveOpenScenes();
                    EditorSceneManager.OpenScene(scene.path);
                }
                else if (option == 1)
                {
                    // Changer sans sauvegarder
                    EditorSceneManager.OpenScene(scene.path);
                }
                // Si "Annuler", ne rien faire
            }
            else
            {
                // Scène non modifiée : changer directement
                EditorSceneManager.OpenScene(scene.path);
            }
        }
        GUILayout.Space(2);

        if (containsMouse)
        {
            Rect toggleRect = new Rect(buttonRect.xMax + 2, rowRect.y, rowRect.width - buttonWidth - 11, rowRect.height);

            if (GUI.Button(toggleRect, scene.isFavorite ? "★" : "☆"))
            {
                // prepare undo to revert the change
                bool previousFavorite = scene.isFavorite;
                List<SceneBrowerData> fromList = previousFavorite ? favoriteScenes : othersScenes;
                List<SceneBrowerData> toList = previousFavorite ? othersScenes : favoriteScenes;
                undoStack.Push(() =>
                {
                    // revert
                    scene.isFavorite = previousFavorite;
                    toList.Remove(scene);
                    if (!fromList.Contains(scene)) fromList.Add(scene);
                });

                // toggle favorite
                scene.isFavorite = !scene.isFavorite;
                // move between lists safely
                favoriteScenes.Remove(scene);
                othersScenes.Remove(scene);
                if (scene.isFavorite) favoriteScenes.Add(scene); else othersScenes.Add(scene);
            }
        }
    }

    static void CheckUndo(Event e)
    {
        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
        {
            if (undoStack.Count > 0)
            {
                undoStack.Pop().Invoke();
                e.Use(); // empêche la propagation
            }
        }
    }

    static void SaveScenesData()
    {
        SceneBrowerDataList list = new SceneBrowerDataList(scenesDataSaved.Values);
        string json = JsonUtility.ToJson(list, true);
        EditorPrefs.SetString(SaveKey, json);
    }
    static void LoadScenesData()
    {
        scenesDataSaved.Clear();

        string json = EditorPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            SceneBrowerDataList list = JsonUtility.FromJson<SceneBrowerDataList>(json);
            if (list.scenes != null)
            {
                foreach (SceneBrowerData scene in list.scenes)
                    scenesDataSaved.Add(scene.path, scene);
            }
        }
    }

    public static Vector2 GetWindowSize(float maxHeight)
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
        }

        if (favoriteScenes.Count > 0 && othersScenes.Count > 0)
            offset += 25;

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