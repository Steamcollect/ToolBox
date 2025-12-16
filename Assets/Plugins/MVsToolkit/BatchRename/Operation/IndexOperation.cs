using System;
using System.Text;

namespace MVsToolkit.BatchRename
{
    /// <summary>
    /// Operation that adds a numeric index or letter to the name.
    /// Supports multiple formats: Number (0, 1, 2...), PaddedNumber (00, 01, 02...), and Letter (A, B, C...).
    /// </summary>
    [Serializable]
    public class IndexOperation : IRenameOperation
    {
        /// <summary>
        /// The format type for the index.
        /// </summary>
        public IndexFormatType FormatType = IndexFormatType.Number;

        /// <summary>
        /// The position where the index should be inserted.
        /// </summary>
        public IndexPosition Position = IndexPosition.Prefix;

        /// <summary>
        /// Optional separator between the index and the name (e.g., "_", "-", "").
        /// </summary>
        public string Separator = "_";

        /// <summary>
        /// The starting index for numbering. Default is 0.
        /// </summary>
        public int StartIndex = 0;

        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            if (ctx == null)
                return false;

            try
            {
                string indexString = GenerateIndexString(ctx.GlobalIndex);
                string insertValue = indexString + Separator;

                if (Position == IndexPosition.Prefix)
                {
                    original.Insert(0, insertValue);
                }
                else if (Position == IndexPosition.Suffix)
                {
                    original.Append(Separator);
                    original.Append(indexString);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Generates the index string based on the format type and the given index.
        /// </summary>
        private string GenerateIndexString(int index)
        {
            int adjustedIndex = index + StartIndex;

            return FormatType switch
            {
                IndexFormatType.Number => adjustedIndex.ToString(),
                IndexFormatType.PaddedNumber => adjustedIndex.ToString("D2"),
                IndexFormatType.Letter => IndexToLetter(adjustedIndex),
                _ => adjustedIndex.ToString()
            };
        }

        /// <summary>
        /// Converts a numeric index to a letter (0=A, 1=B, ..., 25=Z, 26=AA, ...).
        /// </summary>
        private string IndexToLetter(int index)
        {
            if (index < 0)
                return "A";

            StringBuilder result = new StringBuilder();
            int letterIndex = index;

            do
            {
                result.Insert(0, (char)('A' + letterIndex % 26));
                letterIndex = letterIndex / 26 - 1;
            } while (letterIndex >= 0);

            return result.ToString();
        }
    }

    /// <summary>
    /// Enumeration for index format types.
    /// </summary>
    public enum IndexFormatType
    {
        /// <summary>
        /// Simple number format: 0, 1, 2, 3, ...
        /// </summary>
        Number,

        /// <summary>
        /// Padded number format with leading zeros: 00, 01, 02, 03, ...
        /// </summary>
        PaddedNumber,

        /// <summary>
        /// Letter format: A, B, C, ..., Z, AA, AB, ...
        /// </summary>
        Letter
    }

    /// <summary>
    /// Enumeration for index position.
    /// </summary>
    public enum IndexPosition
    {
        /// <summary>
        /// Add the index at the beginning of the name.
        /// </summary>
        Prefix,

        /// <summary>
        /// Add the index at the end of the name.
        /// </summary>
        Suffix
    }
}

