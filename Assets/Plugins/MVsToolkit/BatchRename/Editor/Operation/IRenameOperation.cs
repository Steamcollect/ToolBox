namespace MVsToolkit.BatchRename
{
    public interface IRenameOperation 
    {
        string Apply(string original, RenameContext ctx);
    }
}