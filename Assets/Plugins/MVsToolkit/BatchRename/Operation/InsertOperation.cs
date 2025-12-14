using System;

namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class InsertOperation : IRenameOperation
    {
        public string Search;
        public string Text;
        public bool After;
        
        public string Apply(string original, RenameContext ctx)
        {
            int index = original.IndexOf(Search, StringComparison.Ordinal);
            if (index < 0) return original;
            
            if (After)
            {
                index += Search.Length;
            }
            return original.Insert(index, Text);
        }
    }
}