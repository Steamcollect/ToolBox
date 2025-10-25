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
        foreach (var sceneEntry in ScenesData)
        {
            string sceneId = sceneEntry.Key;

            // Resolve the SceneAsset from the stored GlobalObjectId string
            SceneAsset sceneAsset = ResolveSceneAssetFromId(sceneId);
            if (sceneAsset == null) continue;

            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            if (string.IsNullOrEmpty(scenePath)) continue;

            string dataPath = System.IO.Path.ChangeExtension(scenePath, "_HierarchyData.asset");

            SceneHierarchyMeta meta = AssetDatabase.LoadAssetAtPath<SceneHierarchyMeta>(dataPath);
            if (meta == null)
            {
                meta = ScriptableObject.CreateInstance<SceneHierarchyMeta>();
                AssetDatabase.CreateAsset(meta, dataPath);
            }

            meta.items.Clear();
            meta.items.AddRange(sceneEntry.Value.Values);

            EditorUtility.SetDirty(meta);
        }

        AssetDatabase.SaveAssets();
    }
    static SceneAsset ResolveSceneAssetFromId(string sceneId)
    {
        if (string.IsNullOrEmpty(sceneId)) return null;
        if (!GlobalObjectId.TryParse(sceneId, out var gid)) return null;

        return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid) as SceneAsset;
    }

    static void LoadGOData()
    {
        ScenesData.Clear();

        var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (!activeScene.IsValid()) return;

        // We still use your existing scene ID (GlobalObjectId as string)
        string currentSceneId = GetCurrentSceneGlobalId().ToString(); // from your class

        // Resolve the SceneAsset from that ID
        SceneAsset sceneAsset = ResolveSceneAssetFromId(currentSceneId);
        if (sceneAsset == null) return;

        string dataPath = System.IO.Path.ChangeExtension(activeScene.path, "_HierarchyData.asset");
        var meta = AssetDatabase.LoadAssetAtPath<SceneHierarchyMeta>(dataPath);
        if (meta == null) return;

        var sceneDict = new Dictionary<int, GameObjectHierarchyData>();

        foreach (var item in meta.items)
        {
            if (item == null || item.IsGlobalIdEmpty())
                continue;

            if (!GlobalObjectId.TryParse(item.globalID, out var id))
                continue;

            var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
            if (obj == null)
                continue;

            int instanceID = obj.GetInstanceID();
            var newItem = new GameObjectHierarchyData(instanceID, obj, false);

            if (!string.IsNullOrEmpty(item.iconName))
                newItem.SetIcon(item.iconName);

            sceneDict[instanceID] = newItem;
        }

        ScenesData[currentSceneId] = sceneDict;
        EditorApplication.RepaintHierarchyWindow();
    }
}