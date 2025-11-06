using UnityEngine;

namespace ToolBox.Dev
{
    /// <summary>
    /// Crée un onglet dans le component qui englobe toutes les variables jusqu'au prochain Tab
    /// </summary>
    public class TabAttribute : PropertyAttribute
    {
        public string tabName;

        /// <summary>
        /// Nom de l'onglet
        /// </summary>
        /// <param name="tabName"></param>
        public TabAttribute(string tabName)
        {
            this.tabName = tabName;
        }
    }
}