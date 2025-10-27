using UnityEngine;

namespace ToolBox.Dev
{
    /// <summary>
    /// Cree un bouton en dessous de la variable
    ///
    /// <para>
    /// Ne peux pas �tre utlilis� plusieurs fois sur la m�me variable
    /// </para>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public readonly object[] Parameters;

        /// <param name="parameters">Valeurs � passer ou noms de champs</param>
        public ButtonAttribute(params object[] parameters)
        {
            Parameters = parameters;
        }
    }
}