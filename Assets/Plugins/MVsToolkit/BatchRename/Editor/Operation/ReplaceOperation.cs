namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class ReplaceOperation : IRenameOperation
    {
        public string Search;
        public string Replacement;
        
        public string Apply(string original, RenameContext ctx)
        {
            return original.Replace(Search, Replacement);
        }
    }
}