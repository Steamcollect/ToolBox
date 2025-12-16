using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public class RenameContext
    {
        public string AssetPath;
        public int GlobalIndex;
        public int IndexInParent;

        public bool IsAsset;
        public string ParentName;

        public string SceneName;
        public Object TargetObject;
        public int TotalCount;

        /// <summary>
        ///     Validates that critical properties in the context are not null.
        /// </summary>
        /// <returns>True if validation passes, false otherwise.</returns>
        public bool Validate()
        {
            // Check that at least AssetPath is set (for assets) or TargetObject exists (for scene objects)
            if (string.IsNullOrEmpty(AssetPath) && TargetObject == null) return false;

            return true;
        }
    }
}