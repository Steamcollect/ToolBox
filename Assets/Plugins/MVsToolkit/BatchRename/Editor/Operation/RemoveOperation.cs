namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class RemoveOperation : IRenameOperation
    {
        public string Search;
        
        public string Apply(string original, RenameContext ctx)
        {
            return original.Replace(Search, string.Empty);
        }
    }
}