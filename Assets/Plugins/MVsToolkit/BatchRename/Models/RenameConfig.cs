using UnityEngine;

namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class RenameConfig
    {
        [SerializeReference, Dev.SerializeReferenceDrawer] public IRenameOperation[] Operations;
        [SerializeReference, Dev.SerializeReferenceDrawer] public IRenameRule[] Rules;
        
        public bool UseHierarchyOrdering;
        public bool UseBreadthFirstOrdering;
        public bool AutoPadding;

        public int NumberStart = 0;
        public int NumberStep = 1;
        public int NumberPadding = 2;
    }
}