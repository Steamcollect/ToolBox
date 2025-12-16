using System.Collections.Generic;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public static class HierarchyHelper
    {
        public static string GetHierarchyPath(GameObject go, bool includeScene = false)
        {
            if (!go) return string.Empty;

            var parts = new List<string>(10);
            Transform t = go.transform;
            while (t)
            {
                parts.Add(t.name);
                t = t.parent;
            }

            parts.Reverse();
            string path = string.Join("/", parts);

            if (!includeScene) return path;
            string sceneName = go.scene.IsValid() ? go.scene.name : "Scene";
            path = sceneName + "/" + path;

            return path;
        }
    }
}