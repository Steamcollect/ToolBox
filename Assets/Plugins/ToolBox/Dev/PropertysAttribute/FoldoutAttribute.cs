using UnityEngine;

namespace ToolBox.Dev
{
    public class FoldoutAttribute : PropertyAttribute
    {
        public string foldoutName;

        /// <summary>
        /// Nom du foldout
        /// </summary>
        /// <param name="tabName"></param>
        public FoldoutAttribute(string foldoutName)
        {
            this.foldoutName = foldoutName;
        }
    }
}
