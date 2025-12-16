using System;
using System.Text;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class InsertOperation : IRenameOperation
    {
        public string Search;
        public string Text;
        public bool After;
        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            if (string.IsNullOrEmpty(Search) || string.IsNullOrEmpty(Text))
                return false;

            try
            {
                int index = original.ToString().IndexOf(Search, StringComparison.Ordinal);

                if (index < 0)
                    return false;

                if (After) index += Search.Length;

                if (index < 0 || index > original.Length)
                    return false;

                original.Insert(index, Text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}