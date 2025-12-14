namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class SuffixOperation : IRenameOperation
    {
        public string Suffix;
        
        public string Apply(string original, RenameContext ctx)
        {
            return $"{original}{Suffix}";
        }
    }
}