namespace MVsToolkit.BatchRename
{
    public class RenameContext
    {
        public int TotalCount;
        public int GlobalIndex;
        public int IndexInParent;
        public string ParentName;
        
        public string SceneName;
        public string AssetPath;
        
        public bool IsAsset;
        public UnityEngine.Object TargetObject;
    }
}