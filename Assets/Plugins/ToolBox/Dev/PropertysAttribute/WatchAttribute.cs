using UnityEngine;

namespace ToolBox.Dev
{
    /// <summary>
    /// Affiche la variable et sa valeur à l'écran pendant l'exécution
    ///
    /// <para>
    /// Ne peux pas être utlilisé plusieurs fois sur la même variable
    /// </para>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class WatchAttribute : PropertyAttribute { }
}