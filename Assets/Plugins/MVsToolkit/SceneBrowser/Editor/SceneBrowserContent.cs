using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVsToolkit.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MVsToolkit.SceneBrower
{
    [InitializeOnLoad]
    public static class SceneBrowserContent
    {
        static List<SceneBrowerData> scenes = new();
        const string SaveKey = "SceneBrowser_Data";

        static List<SceneBrowerData> favoriteScenes = new(), otherScenes = new();

        static GUIStyle buttonStyle;

        static SceneBrowserContent()
        {
            RefreshScenesList();
            InitButtonStyle();
        }

        #region Drawing
        public static void OnGUI(Rect rect, string searchQuery)
        {
            foreach (var scene in GetScenesWithQuery(searchQuery))
            {
                DrawSceneItem(scene);
            }
        }

        static void DrawSceneItem(SceneBrowerData sceneData)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(sceneData.asset.name, buttonStyle))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneData.asset));
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public static Vector2 GetWindowSize()
        {
            return new Vector2(250, 300);
        }
        #endregion

        #region Data Management
        public static void RefreshScenesList()
        {
            LoadScenesData();

            favoriteScenes.Clear();
            otherScenes.Clear();

            SceneAsset[] allScenes = GetAllScenesInAssetsRoot();

            foreach (SceneAsset scene in allScenes) // Add new scenes
                if (scenes.Find(s => s.asset == scene) == null)
                    scenes.Add(new SceneBrowerData(scene, false));

            SceneBrowerData[] scenesArray = scenes.ToArray();
            foreach (SceneBrowerData scene in scenesArray) // Remove deleted scenes
            {
                if (!allScenes.Contains(scene.asset))
                    scenes.Remove(scene);

                if(scene.isFavorite)
                    favoriteScenes.Add(scene);
                else
                    otherScenes.Add(scene);
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

        #region GUI Style
        static void InitButtonStyle()
        {
            buttonStyle = new GUIStyle(EditorStyles.label);

            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.padding = new RectOffset(5, 0, 0, 0);
            buttonStyle.fontSize = 12;

            buttonStyle.normal.background = null;

            buttonStyle.hover.background = TextureUtils.MakeColorTex(1, 1, new Color(0.24f, 0.48f, 0.90f, 0.6f));
            buttonStyle.hover.textColor = Color.white;

            buttonStyle.normal.textColor = Color.white;
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