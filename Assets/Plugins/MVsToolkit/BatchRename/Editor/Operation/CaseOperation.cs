namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class CaseOperation : IRenameOperation
    {
        
        public CaseType Type;
        
        public enum CaseType
        {
            Upper,
            Lower
        }
        
        public string Apply(string original, RenameContext ctx)
        {
            return Type == CaseType.Upper ? original.ToUpper() : original.ToLower();
        }
    }
}