namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class RenameConfig
    {
        public IRenameOperation[] Operations;
        public IRenameRule[] Rules;


        public bool UseHierarchyOrdering;
        public bool UseBreadthFirstOrdering;
        public bool AutoPadding;

        public int NumberStart = 0;
        public int NumberStep = 1;
        public int NumberPadding = 2;
    }
}