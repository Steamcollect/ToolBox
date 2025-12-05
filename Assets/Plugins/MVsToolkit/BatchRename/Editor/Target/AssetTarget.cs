namespace MVsToolkit.BatchRename
{
    public class AssetTarget : IRenameTarget
    {
        public string Name { get; }
        public string Path { get; }
        public UnityEngine.Object UnityObject { get; }
        
        private string m_AssetPath;
        private UnityEngine.Object m_AssetObject;

        public AssetTarget(string assetPath)
        {
            m_AssetObject = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPath);
            m_AssetPath = assetPath;
        }
        
        public void SetName(string newName) => UnityEditor.AssetDatabase.RenameAsset(m_AssetPath, newName);
    }
}