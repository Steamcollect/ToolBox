using System;
using System.Text;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class SuffixOperation : IRenameOperation
    {
        public string Suffix;
        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            if (Suffix == null)
                return false;

            try
            {
                original.Append(Suffix);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}