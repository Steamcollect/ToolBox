using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public class AssetTarget : IRenameTarget
    {
        public AssetTarget(string assetPath)
        {
            UnityObject = AssetDatabase.LoadMainAssetAtPath(assetPath);
            Path = assetPath;
        }

        public string Name => UnityObject.name;
        public string Path { get; }
        public Object UnityObject { get; }

        public void SetName(string newName)
        {
            AssetDatabase.RenameAsset(Path, newName);
        }

        public bool IsValidTarget()
        {
            return UnityObject != null;
        }
    }
}