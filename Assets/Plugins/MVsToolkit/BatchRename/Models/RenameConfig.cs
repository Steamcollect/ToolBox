using System;
using MVsToolkit.Dev;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class RenameConfig
    {
        [SerializeReference] [SerializeReferenceDrawer]
        public IRenameOperation[] Operations;

        [SerializeReference] [SerializeReferenceDrawer]
        public IRenameRule[] Rules;
    }
}