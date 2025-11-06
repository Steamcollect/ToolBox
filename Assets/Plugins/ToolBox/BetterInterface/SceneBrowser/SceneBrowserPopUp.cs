#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ToolBox.BetterInterface
{
    public class SceneBrowserPopUp : PopupWindowContent
    {
        int buttonHeight = 20;
        int searchBarHeight = 22;
        int maxHeight = 300;

        private Vector2 scrollPos;
        private string searchQuery = "";

        Dictionary<string /*path*/, SceneBrowerData> scenesDataSaved = new();
        private const string SaveKey = "SceneBrowser_Data";

        List<SceneBrowerData> favoriteScenes = new(), othersScenes = new();

        Stack<System.Action> undoStack = new();

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

        GUIStyle leftButtonStyle, RightTextStyle;
        void SetStyles()
        {
            leftButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 2, 2),
            };

            RightTextStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 2, 2),
            };
        }

        public override void OnOpen()
        {
            SetStyles();

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
            DrawButtons(e);
            CheckUndo(e);
        }

        void DrawButtons(Event e)
        {
            // --- ZONE SCROLLABLE ---
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if (favoriteScenes.Count > 0)
            {
                foreach (SceneBrowerData scene in favoriteScenes)
                {
                    // Filtrage de la recherche
                    if (!string.IsNullOrEmpty(searchQuery) &&
                        !scene.name.ToLower().Contains(searchQuery.ToLower()))
                    {
                        continue;
                    }

                    Rect rowRect = GUILayoutUtility.GetRect(250, buttonHeight, GUILayout.ExpandWidth(true)); // Get space in GUILayout

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

                        if (GUI.Button(toggleRect, "★"))
                        {
                            undoStack.Push(() =>
                            {
                                scene.isFavorite = true;
                                favoriteScenes.Add(scene);
                                othersScenes.Remove(scene);
                            });

                            scene.isFavorite = false;
                            othersScenes.Add(scene);
                            favoriteScenes.Remove(scene);
                            break;
                        }
                    }
                    else
                    {
                        GUI.Label(buttonRect, "★", RightTextStyle);
                    }
                }
            }


            if (favoriteScenes.Count > 0 && othersScenes.Count > 0)
            {
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

                    if (GUI.Button(toggleRect, "☆"))
                    {
                        undoStack.Push(() =>
                        {
                            scene.isFavorite = false;
                            othersScenes.Add(scene);
                            favoriteScenes.Remove(scene);
                        });

                        scene.isFavorite = true;
                        othersScenes.Remove(scene);
                        favoriteScenes.Add(scene);
                        break;
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        void CheckUndo(Event e)
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
                        scenesDataSaved.Add(scene.path, scene);
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
}
#endif