using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVsToolkit.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MVsToolkit.SceneBrowser
{
    [InitializeOnLoad]
    public static class SceneBrowserContent
    {
        static List<SceneBrowerData> scenes = new();
        const string SaveKey = "SceneBrowser_Data";

        static int buttonHeight = 18;
        static int buttonSpacing = 2;

        static float panelHeight;

        static GUIStyle sceneButtonStyle, favoriteButtonStyle;

        static SceneBrowserContent()
        {
            RefreshScenesList();
        }

        #region Drawing
        public static void OnGUI(Rect rect, string searchQuery)
        {
            EnsureButtonStyle();

            Event e = Event.current;

            SceneBrowerData[] _scenes = GetScenesWithQuery(searchQuery);
            _scenes = _scenes.OrderBy(c => !c.isFavorite).ToArray();

            //bool useScrollView = rect.height < _scenes.Length * buttonHeight + _scenes.Length * buttonSpacing;

            float currentHeight = 0;

            for (int i = 0; i < _scenes.Length; i++)
            {
                if (!_scenes[i].isFavorite && i - 1 >= 0 && scenes[i - 1].isFavorite)
                {
                    EditorGUI.DrawRect(new Rect(rect.x + rect.width * .05f, rect.y + currentHeight + 4, rect.width * .9f, 1), Color.grey);
                    currentHeight += 9;
                }

                Rect buttonRect = new Rect(rect.x, rect.y + currentHeight, rect.width, buttonHeight);

                bool mouseInButton = buttonRect.Contains(e.mousePosition);
                if (mouseInButton)
                    EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height), new Color(0.172549f, 0.3647059f, 0.5294118f));
                else if (i % 2 == 0)
                    EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height), new Color(1, 1, 1, 0.02f));
                
                DrawSceneItem(_scenes[i], buttonRect, e, mouseInButton);

                currentHeight += buttonHeight + buttonSpacing;
            }

            panelHeight = currentHeight;
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
                    SaveScenesData();
                }
            }
            
            if (GUI.Button(r, sceneData.asset.name, sceneButtonStyle))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
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
        public bool isFavorite;

        public SceneBrowerData(SceneAsset scene, bool isFavorite)
        {
            this.asset = scene;
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