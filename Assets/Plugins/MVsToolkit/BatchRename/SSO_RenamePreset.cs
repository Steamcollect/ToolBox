using UnityEngine;

namespace MVsToolkit.BatchRename
{
    [CreateAssetMenu(fileName = "SSO_RenamePreset", menuName = "MVsToolkit/BatchRename/RenamePreset")]
    public class SSO_RenamePreset : ScriptableObject
    {
        public RenameConfig Config = new();
    }
}
