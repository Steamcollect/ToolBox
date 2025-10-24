using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameObjectHierarchyEditor
{
    static Dictionary<int /*instanceID*/, GameObjectHierarchyData> gamObjectsData = new();
    private const string SaveKey = "GameObjectCustomizer_Data";


    static GameObjectHierarchyEditor()
    {
        LoadGOData();

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
        gamObjectsData[instanceID].SetIcon(iconName);
        SaveGOData();

        EditorApplication.RepaintHierarchyWindow();
    }

    static GameObjectHierarchyData GetOrCreateGOHierarchyData(int instanceId)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceId);
        if (obj == null) return null;

        if (!gamObjectsData.TryGetValue(instanceId, out GameObjectHierarchyData go))
        {
            go = new GameObjectHierarchyData(instanceId, obj as GameObject);
            gamObjectsData[instanceId] = go;
        }

        return go;
    }

    public static void SaveGOData()
    {
        GameObjectHierarchyList list = new GameObjectHierarchyList(gamObjectsData.Values);
        string json = JsonUtility.ToJson(list, true);
        EditorPrefs.SetString(SaveKey, json);
    }

    static void LoadGOData()
    {
        gamObjectsData.Clear();
        string json = EditorPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            var list = JsonUtility.FromJson<GameObjectHierarchyList>(json);
            if (list?.items != null)
            {
                foreach (var item in list.items)
                    gamObjectsData[item.InstanceID] = item;
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