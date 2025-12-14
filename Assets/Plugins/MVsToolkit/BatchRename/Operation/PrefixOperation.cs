namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class PrefixOperation : IRenameOperation
    {
        public string Prefix;
        
        public string Apply(string original, RenameContext ctx)
        {
            return $"{Prefix}{original}";
        }
    }
}