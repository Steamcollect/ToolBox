using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameObjectHierarchyEditor
{
    static Dictionary<int /*instanceID*/, GameObjectHierarchyData> objectsData = new();
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

        // Détermine la couleur du fond selon le thème et la sélection
        bool isSelected = Selection.instanceIDs.Contains(instanceID);
        Color bgColor = isSelected
            ? new Color(0.172549f, .3647059f, .5294118f)
            : (EditorGUIUtility.isProSkin
                ? new Color(0.219f, 0.219f, 0.219f)
                : new Color(0.76f, 0.76f, 0.76f));

        Rect iconRect = new Rect(selectionRect.x - 1, selectionRect.y, 16, 16);
        EditorGUI.DrawRect(iconRect, bgColor);
        GUI.DrawTexture(iconRect, data.icon, ScaleMode.ScaleToFit);
    }

    public static GameObjectHierarchyData GetData(int instanceID)
    {
        return GetOrCreateGOHierarchyData(instanceID);
    }
    public static void SetData(int instanceID, Texture2D icon)
    {
        objectsData[instanceID].icon = icon;
        SaveGOData();

        EditorApplication.RepaintHierarchyWindow();
    }

    static GameObjectHierarchyData GetOrCreateGOHierarchyData(int instanceId)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceId);
        if (obj == null) return null;

        if (!objectsData.TryGetValue(instanceId, out GameObjectHierarchyData go))
        {
            go = new GameObjectHierarchyData(instanceId, obj as GameObject);
            objectsData[instanceId] = go;
        }

        return go;
    }

    public static void SaveGOData()
    {
        GameObjectHierarchyList list = new GameObjectHierarchyList(objectsData.Values);
        string json = JsonUtility.ToJson(list, true);
        EditorPrefs.SetString(SaveKey, json);
    }

    static void LoadGOData()
    {
        objectsData.Clear();
        string json = EditorPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            var list = JsonUtility.FromJson<GameObjectHierarchyList>(json);
            if (list?.items != null)
            {
                foreach (var item in list.items)
                    objectsData[item.InstanceID] = item;
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