using System;
using System.Text;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class ReplaceOperation : IRenameOperation
    {
        public string Search;
        public string Replacement;
        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            if (string.IsNullOrEmpty(Search))
                return false;

            try
            {
                string replacement = Replacement ?? string.Empty;

                original.Replace(Search, replacement);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}