using System;
using System.Text;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    [Serializable]
    public class CaseOperation : IRenameOperation
    {
        public enum CaseType
        {
            Upper,
            Lower
        }

        public CaseType Type;
        public bool IsEnabled { get; set; } = true;

        public bool Apply(StringBuilder original, RenameContext ctx)
        {
            if (!IsEnabled)
                return false;

            try
            {
                switch (Type)
                {
                    case CaseType.Upper:
                    {
                        for (int i = 0; i < original.Length; i++) original[i] = char.ToUpper(original[i]);

                        break;
                    }
                    case CaseType.Lower:
                    {
                        // Convert to lowercase in-place
                        for (int i = 0; i < original.Length; i++) original[i] = char.ToLower(original[i]);

                        break;
                    }
                    default:
                        Debug.LogWarning($"{typeof(CaseOperation)} Unknown CaseType: {Type}");
                        break;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}