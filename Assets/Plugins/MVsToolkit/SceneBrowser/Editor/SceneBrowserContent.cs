using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.SceneBrowser
{
    [InitializeOnLoad]
    public static class SceneBrowserContent
    {
        static List<SceneBrowerData> scenes = new();
        const string SaveKey = "SceneBrowser_Data";

        static GUIStyle sceneButtonStyle, favoriteButtonStyle;
        
        static int buttonHeight = 18;
        static int buttonSpacing = 2;
        static int scrollbarWidth = 8;

        static float panelHeight;

        static Vector2 scrollPos;

        static SceneBrowserContent()
        {
            RefreshScenesList();
        }

        #region Drawing
        public static void DrawContent(Rect rect, string searchQuery, float maxContentHeight)
        {
            EnsureButtonStyle();

            Event e = Event.current;

            SceneBrowerData[] _scenes = GetScenesWithQuery(searchQuery);

            bool needScroll = panelHeight > maxContentHeight;

            float innerWidth = rect.width - (needScroll ? scrollbarWidth : 0) - 2;
            Rect viewRect = new Rect(0, 0, innerWidth, panelHeight);

            float currentHeight = 0;
            for (int i = 0; i < _scenes.Length; i++)
            {
                if (!_scenes[i].isFavorite && i - 1 >= 0 && _scenes[i - 1].isFavorite)
                    currentHeight += 9;

                currentHeight += buttonHeight;

                if (i < _scenes.Length - 1)
                    currentHeight += buttonSpacing;
            }

            panelHeight = currentHeight;


            scrollPos = GUI.BeginScrollView(
                rect,
                scrollPos,
                new Rect(0,0,viewRect.width - 10, viewRect.height),
                false,
                needScroll
            );

            currentHeight = 0;
            for (int i = 0; i < _scenes.Length; i++)
            {
                if (_scenes[i].asset == null)
                {
                    scenes.Remove(_scenes[i]);
                    continue;
                }

                if (!_scenes[i].isFavorite && i - 1 >= 0 && _scenes[i - 1].isFavorite)
                {
                    EditorGUI.DrawRect(new Rect(10, currentHeight + 4, innerWidth - 20, 1), Color.grey);
                    currentHeight += 9;
                }

                Rect buttonRect = new Rect(0, currentHeight, innerWidth, buttonHeight);

                bool mouseInButton = buttonRect.Contains(e.mousePosition);

                if (mouseInButton)
                    EditorGUI.DrawRect(buttonRect, new Color(0.172549f, 0.3647059f, 0.5294118f));
                else if (i % 2 == 0)
                    EditorGUI.DrawRect(buttonRect, new Color(1, 1, 1, 0.02f));

                DrawSceneItem(_scenes[i], buttonRect, e, mouseInButton);

                currentHeight += buttonHeight + buttonSpacing;
            }

            GUI.EndScrollView();
        }

        static void DrawSceneItem(SceneBrowerData sceneData, Rect r, Event e, bool mouseInButton)
        {
            if(sceneData.isFavorite || mouseInButton)
            {
                Rect favoriteRect = new Rect(r.x + r.width - r.height * 2, r.y, r.height * 2, r.height);

                bool favoriteContainMouse = favoriteRect.Contains(e.mousePosition);
                string favoriteButtonText = "☆";

                if (sceneData.isFavorite || (!sceneData.isFavorite && favoriteContainMouse)) favoriteButtonText = "★";
                if (!sceneData.isFavorite || (sceneData.isFavorite && favoriteContainMouse)) favoriteButtonText = "☆";

                if (GUI.Button(favoriteRect, favoriteButtonText, favoriteButtonStyle))
                {
                    sceneData.isFavorite = !sceneData.isFavorite;
                    scenes = scenes.OrderBy(scenes => !scenes.isFavorite).ToList();
                    SaveScenesData();
                }
            }
            
            if (GUI.Button(r, sceneData.asset == null ? sceneData.assetName : sceneData.asset.name, sceneButtonStyle))
                {
                    if (sceneData.asset != null && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneData.asset));
                    }
                }
        }

        public static Vector2 GetWindowSize()
        {
            return new Vector2(250, panelHeight);
        }

        static void EnsureButtonStyle()
        {
            if (sceneButtonStyle != null) return;

            sceneButtonStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                padding = new RectOffset(10, 0, 0, 0)
            };

            favoriteButtonStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = buttonHeight,
                padding = new RectOffset(0, 10, 0, 0)
            };
        }
        #endregion

        #region Data Management
        public static void RefreshScenesList()
        {
            LoadScenesData();

            SceneAsset[] allScenes = GetAllScenesInAssetsRoot();

            foreach (SceneAsset scene in allScenes) // Add new scenes
                if (scenes.Find(s => s.asset == scene) == null)
                    scenes.Add(new SceneBrowerData(scene, false));

            SceneBrowerData[] scenesArray = scenes.ToArray();
            foreach (SceneBrowerData scene in scenesArray) // Remove deleted scenes
            {
                if (!allScenes.Contains(scene.asset))
                    scenes.Remove(scene);
            }

            scenes = scenes.OrderBy(scenes => !scenes.isFavorite).ToList();
        }

        public static void SaveScenesData()
        {
            SceneBrowerDataList list = new SceneBrowerDataList(scenes);
            string json = JsonUtility.ToJson(list, true);
            EditorPrefs.SetString(SaveKey, json);
        }
        static void LoadScenesData()
        {
            scenes.Clear();

            string json = EditorPrefs.GetString(SaveKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                SceneBrowerDataList list = JsonUtility.FromJson<SceneBrowerDataList>(json);
                if (list.scenes != null)
                    foreach (SceneBrowerData scene in list.scenes)
                        scenes.Add(scene);
            }
        }

        public static SceneAsset[] GetAllScenesInAssetsRoot()
        {
            // Get all project scene
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

            List<SceneAsset> scenes = new();

            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Check if the scene is in the Assets root folder
                if (path.StartsWith("Assets/"))
                {
                    SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                    if (scene != null) scenes.Add(scene);
                }
            }

            return scenes.ToArray();
        }

        static SceneBrowerData[] GetScenesWithQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
                return scenes.ToArray();
            else
                return scenes.Where(s => s.asset.name.ToLower().Contains(query.ToLower())).ToArray();
        }
        #endregion
    }

    [System.Serializable]
    class SceneBrowerData
    {
        public SceneAsset asset;
        public string assetName;
        public bool isFavorite;

        public SceneBrowerData(SceneAsset scene, bool isFavorite)
        {
            this.asset = scene;
            this.assetName = scene.name;
            this.isFavorite = isFavorite;
        }
    }

    [System.Serializable]
    class SceneBrowerDataList
    {
        public List<SceneBrowerData> scenes;
        public SceneBrowerDataList(IEnumerable<SceneBrowerData> data)
        {
            scenes = new List<SceneBrowerData>(data);
        }
    }
}