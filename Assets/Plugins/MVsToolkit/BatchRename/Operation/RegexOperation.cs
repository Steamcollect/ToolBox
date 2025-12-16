using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class RegexOperation : IRenameOperation
    {
        public string Pattern;
        public string Replacement;
        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            if (string.IsNullOrEmpty(Pattern))
                return false;

            try
            {
                string replacement = Replacement ?? string.Empty;
                string result = Regex.Replace(original.ToString(), Pattern, replacement);

                // Clear and rebuild StringBuilder with result
                original.Clear();
                original.Append(result);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}