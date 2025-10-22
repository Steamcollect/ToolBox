namespace ToolBox.Debug
{
    using UnityEngine;

    /// <summary>
    /// Affiche la variable et sa valeur � l'�cran pendant l'ex�cution
    ///
    /// <para>
    /// Ne peux pas �tre utlilis� plusieurs fois sur la m�me variable
    /// </para>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class WatchAttribute : PropertyAttribute { }
}