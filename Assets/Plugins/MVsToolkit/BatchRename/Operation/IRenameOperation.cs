using System.Text;

namespace MVsToolkit.BatchRename
{
    public interface IRenameOperation
    {
       
        bool IsEnabled { get; set; }

        /// <summary>
        ///     Applies the rename operation to the provided StringBuilder.
        /// </summary>
        /// <param name="original">The StringBuilder containing the current name. Must not be null.</param>
        /// <param name="ctx">The rename context. Must not be null.</param>
        /// <returns>True if the operation succeeded, false if it failed or was skipped.</returns>
        bool Apply(StringBuilder original, RenameContext ctx);
    }
}