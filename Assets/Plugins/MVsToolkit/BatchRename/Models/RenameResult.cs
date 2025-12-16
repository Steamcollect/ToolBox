namespace MVsToolkit.BatchRename
{
    public class RenameResult
    {
        public string ErrorMessage;

        public bool HasConflict;
        public bool HasError;
        public string NewName;
        public string OldName;
        public IRenameTarget Target;
    }
}