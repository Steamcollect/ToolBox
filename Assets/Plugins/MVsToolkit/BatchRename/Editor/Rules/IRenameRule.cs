namespace MVsToolkit.BatchRename
{
    public interface IRenameRule
    {
        bool Matches(IRenameTarget target, RenameContext ctx);
    }
}