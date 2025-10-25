using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class GameObjectHierarchyEditor
{
    static Dictionary<string /*SceneID*/, Dictionary<int /*instanceID*/, GameObjectHierarchyData>> ScenesData = new();
    private const string SaveKey = "GameObjectCustomizer_Data";

    static GameObjectHierarchyEditor()
    {
        EditorApplication.delayCall += LoadGOData;
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

        UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;

        UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += OnSceneSaved;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObjectHierarchyData data = GetOrCreateGOHierarchyData(GetCurrentSceneGlobalId().ToString(), instanceID);

        if (data == null || data.GameObject == null)
            return;

        data.Draw(selectionRect);
    }

    static void OnSceneSaved(Scene scene)
    {
        SaveGOData();
    }

    static void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        LoadGOData();
    }

    public static GameObjectHierarchyData GetData(string sceneID, int instanceID)
    {
        return GetOrCreateGOHierarchyData(sceneID, instanceID);
    }
    public static void SetData(string sceneID, int instanceID, string iconName)
    {
        ScenesData[sceneID][instanceID].SetIcon(iconName);

        var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (activeScene.IsValid())
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);

        EditorApplication.RepaintHierarchyWindow();
    }

    static GameObjectHierarchyData GetOrCreateGOHierarchyData(string sceneID, int instanceId)
    {
        if (ScenesData.ContainsKey(sceneID) == false)
            ScenesData.Add(sceneID, new Dictionary<int, GameObjectHierarchyData>());

        if (!ScenesData[sceneID].TryGetValue(instanceId, out GameObjectHierarchyData newGO))
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj == null)
                return null;

            newGO = new GameObjectHierarchyData(instanceId, obj, true);
            ScenesData[sceneID].Add(instanceId, newGO);
        }

        return newGO;
    }

    public static GlobalObjectId GetCurrentSceneGlobalId()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        string scenePath = activeScene.path;

        // Load the SceneAsset
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        if (sceneAsset == null)
            return default;

        return GlobalObjectId.GetGlobalObjectIdSlow(sceneAsset);
    }

    public static void SaveGOData()
    {
        List<SceneDataWrapper> allScenes = new();

        foreach (var sceneEntry in ScenesData)
        {
            SceneDataWrapper wrapper = new SceneDataWrapper
            {
                sceneID = sceneEntry.Key,
                items = sceneEntry.Value.Values.ToList()
            };
            allScenes.Add(wrapper);
        }

        string json = JsonUtility.ToJson(new SceneCollectionWrapper(allScenes), true);
        EditorPrefs.SetString(SaveKey, json);
    }

    static void LoadGOData()
    {
        ScenesData.Clear();

        string json = EditorPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(json))
            return;

        var collection = JsonUtility.FromJson<SceneCollectionWrapper>(json);
        if (collection == null || collection.scenes == null)
            return;

        foreach (var sceneWrapper in collection.scenes)
        {
            if (string.IsNullOrEmpty(sceneWrapper.sceneID))
                continue;

            var sceneDict = new Dictionary<int, GameObjectHierarchyData>();
            ScenesData[sceneWrapper.sceneID] = sceneDict;

            foreach (var item in sceneWrapper.items)
            {
                if (item == null || item.IsGlobalIdEmpty())
                    continue;

                if (!GlobalObjectId.TryParse(item.globalID, out var id))
                    continue;

                var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
                if (obj == null)
                    continue;

                int instanceID = obj.GetInstanceID();
                if (sceneDict.ContainsKey(instanceID))
                    continue;

                var newItem = new GameObjectHierarchyData(instanceID, obj, false);

                if (!string.IsNullOrEmpty(item.iconName)) // Set Icon
                    newItem.SetIcon(item.iconName);

                sceneDict[instanceID] = newItem;
            }
        }

        EditorApplication.RepaintHierarchyWindow();
    }

    [Serializable]
    class SceneCollectionWrapper
    {
        public List<SceneDataWrapper> scenes = new();

        public SceneCollectionWrapper(List<SceneDataWrapper> list)
        {
            scenes = list;
        }

        // For JsonUtility
        public SceneCollectionWrapper() { }
    }
    [Serializable]
    class SceneDataWrapper
    {
        public string sceneID;
        public List<GameObjectHierarchyData> items = new();
    }

    [MenuItem("ToolBox/Better Window/Clear GameObjects Datas")]
    public static void ClearSavedData()
    {
        // Clear in-memory data
        ScenesData.Clear();

        // Remove saved JSON from EditorPrefs
        if (EditorPrefs.HasKey(SaveKey))
            EditorPrefs.DeleteKey(SaveKey);

        // Force Unity to refresh the Hierarchy
        EditorApplication.RepaintHierarchyWindow();

        Debug.Log("<color=orange>[GameObjectsData]</color> Cleared all saved GameObject Datas");
    }
}