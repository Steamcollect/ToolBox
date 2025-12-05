using System;

namespace MVsToolkit.BatchRename
{
    [System.Serializable]
    public class RegexOperation : IRenameOperation
    {
        public string Pattern;
        public string Replacement;
        
        public string Apply(string original, RenameContext ctx)
        {
            try
            {
                return System.Text.RegularExpressions.Regex.Replace(original, Pattern, Replacement);
            }
            catch (Exception)
            {
                return original;
            }
        }
    }
}