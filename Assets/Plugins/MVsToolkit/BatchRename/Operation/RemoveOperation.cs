using System;
using System.Text;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class RemoveOperation : IRenameOperation
    {
        public string Search;
        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            if (string.IsNullOrEmpty(Search))
                return false;

            try
            {
                original.Replace(Search, string.Empty);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}