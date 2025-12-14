using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public class AssetTarget : IRenameTarget
    {
        public string Name => UnityObject.name;
        public string Path { get; }
        public UnityEngine.Object UnityObject { get; }

        public AssetTarget(string assetPath)
        {
            UnityObject = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPath);
            Debug.Log(UnityObject.name);
            Path = assetPath;
        }
        
        public void SetName(string newName) => UnityEditor.AssetDatabase.RenameAsset(Path, newName);
    }
}