using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameObjectHierarchyEditor
{
    static Dictionary<int /*instanceID*/, GameObjectHierarchyData> gameObjectsData = new();
    private const string SaveKey = "GameObjectCustomizer_Data";

    static GameObjectHierarchyEditor()
    {
        EditorApplication.delayCall += LoadGOData;
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObjectHierarchyData data = GetOrCreateGOHierarchyData(instanceID);

        if (data == null || data.GameObject == null)
            return;

        data.Draw(selectionRect);
    }

    public static GameObjectHierarchyData GetData(int instanceID)
    {
        return GetOrCreateGOHierarchyData(instanceID);
    }
    public static void SetData(int instanceID, string iconName)
    {
        gameObjectsData[instanceID].SetIcon(iconName);
        SaveGOData();

        EditorApplication.RepaintHierarchyWindow();
    }

    static GameObjectHierarchyData GetOrCreateGOHierarchyData(int instanceId)
    {
        if (!gameObjectsData.TryGetValue(instanceId, out GameObjectHierarchyData newGO))
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj == null)
                return null;

            newGO = new GameObjectHierarchyData(instanceId, obj, true);
            gameObjectsData.Add(instanceId, newGO);
        }

        return newGO;
    }

    public static void SaveGOData()
    {
        GameObjectHierarchyList list = new GameObjectHierarchyList(gameObjectsData.Values);
        string json = JsonUtility.ToJson(list, true);
        EditorPrefs.SetString(SaveKey, json);
    }

    static void LoadGOData()
    {
        gameObjectsData.Clear();

        string json = EditorPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            var list = JsonUtility.FromJson<GameObjectHierarchyList>(json);
            if (list?.items != null)
            {
                foreach (var item in list.items)
                {
                    if (item.IsGlobalIdEmpty())
                        continue;

                    if(GlobalObjectId.TryParse(item.globalID, out GlobalObjectId id))
                    {
                        UnityEngine.Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
                        int instanceID = obj.GetInstanceID();

                        GameObjectHierarchyData newItem = new GameObjectHierarchyData(instanceID, obj, false);

                        if (gameObjectsData.ContainsKey(instanceID))
                            continue;

                        if (!string.IsNullOrEmpty(item.iconName))
                            newItem.SetIcon(item.iconName);

                        gameObjectsData.Add(instanceID, newItem);
                    }
                }

                EditorApplication.RepaintHierarchyWindow();
            }
        }
    }

    [System.Serializable]
    class GameObjectHierarchyList
    {
        public List<GameObjectHierarchyData> items = new List<GameObjectHierarchyData>();
        public GameObjectHierarchyList(IEnumerable<GameObjectHierarchyData> data)
        {
            items.AddRange(data);
        }
    }
}