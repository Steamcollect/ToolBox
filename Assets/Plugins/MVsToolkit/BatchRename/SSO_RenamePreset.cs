using System;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    [CreateAssetMenu(fileName = "SSO_RenamePreset", menuName = "MVsToolkit/BatchRename/RenamePreset")]
    public class SSO_RenamePreset : ScriptableObject
    {
        public RenameConfig Config = new();


        public static SSO_RenamePreset DefaultPreset()
        {
            IRenameOperation[] operations =
            {
                new IndexOperation()
            };

            IRenameRule[] rules = Array.Empty<IRenameRule>();

            RenameConfig config = new()
            {
                Operations = operations,
                Rules = rules,
            };

            SSO_RenamePreset preset = CreateInstance<SSO_RenamePreset>();
            preset.Config = config;
            return preset;
        }
    }
}