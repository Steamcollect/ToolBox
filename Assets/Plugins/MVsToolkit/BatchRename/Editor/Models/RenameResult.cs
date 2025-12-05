namespace MVsToolkit.BatchRename
{
    public class RenameResult
    {
        public IRenameTarget Target;
        public string OldName;
        public string NewName;

        public bool HasConflict;
        public bool HasError;
        public string ErrorMessage;
    }
}