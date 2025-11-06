using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterInterface
{
    [InitializeOnLoad]
    public static class GameObjectHierarchyEditor
    {
        static Dictionary<int /*instanceID*/, GameObjectHierarchyData> gameObjects = new();

        static GameObjectHierarchyEditor()
        {
            gameObjects.Clear();

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObjectHierarchyData data = GetOrCreateGOHierarchyData(instanceID);

            if (data == null || data.obj == null)
                return;

            data.Draw(selectionRect);
        }

        static GameObjectHierarchyData GetOrCreateGOHierarchyData(int instanceId)
        {
            if (!gameObjects.TryGetValue(instanceId, out GameObjectHierarchyData newGO))
            {
                Object obj = EditorUtility.InstanceIDToObject(instanceId);
                newGO = new GameObjectHierarchyData(obj);

                gameObjects.Add(instanceId, newGO);
            }

            return newGO;
        }
    }
}