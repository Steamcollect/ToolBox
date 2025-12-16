using System;
using System.Text;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class PrefixOperation : IRenameOperation
    {
        public string Prefix;
        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            if (Prefix == null)
                return false;

            try
            {
                original.Insert(0, Prefix);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}