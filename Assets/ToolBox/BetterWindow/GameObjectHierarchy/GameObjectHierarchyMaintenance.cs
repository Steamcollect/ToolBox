#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using System.Linq;

public static class GameObjectHierarchyMaintenance
{
    [MenuItem("ToolBox/Better Window/Cleanup GameObjects Datas")]
    public static void CleanupOrphanedHierarchyData()
    {
        string folderPath = "Assets/ToolBox/Datas/ScenesHierarchy/";
        if (!Directory.Exists(folderPath))
        {
            UnityEngine.Debug.Log("No hierarchy data folder found.");
        }

        string[] assetPaths = AssetDatabase.FindAssets("t:SceneHierarchyMeta", new[] { folderPath })
                                           .Select(AssetDatabase.GUIDToAssetPath)
                                           .ToArray();

        int removed = 0;
        foreach (var path in assetPaths)
        {
            var meta = AssetDatabase.LoadAssetAtPath<SceneHierarchyMeta>(path);
            if (meta == null) continue;

            bool missingScene = string.IsNullOrEmpty(meta.sceneGuid) ||
                                string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(meta.sceneGuid));

            if (missingScene)
            {
                AssetDatabase.DeleteAsset(path);
                removed++;
            }
        }

        AssetDatabase.Refresh();
    }
}
#endif