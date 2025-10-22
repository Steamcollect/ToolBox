namespace ToolBox.Dev
{
    using UnityEngine;

    /// <summary>
    /// Cree un bouton en dessous de la variable
    ///
    /// <para>
    /// Ne peux pas �tre utlilis� plusieurs fois sur la m�me variable
    /// </para>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public readonly string Path;
        public readonly bool ShowVariable;

        /// <param name="path">Nom de l'Action � call</param>
        /// <param name="showVariable">Cacher la variable</param>
        public ButtonAttribute(string path, bool showVariable = true)
        {
            Path = path;
            ShowVariable = showVariable;
        }
    }
}