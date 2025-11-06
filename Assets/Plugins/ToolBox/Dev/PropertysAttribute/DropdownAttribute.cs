using UnityEngine;
using UnityEngine.UI;

namespace ToolBox.Dev
{
    /// <summary>
    /// Affiche un dropdown dans l'inspector avec les valeurs donnée
    /// 
    /// <para>
    /// Les valeurs peuvent être hard codé ou par référence
    /// </para> 
    /// <para>
    /// La variable peut venir d'un script référencé
    /// </para>
    /// 
    /// <para>
    /// Soutient : String, Float, Int
    /// </para>
    ///  
    /// <para>Exemple :</para>
    /// <list type="bullet">
    ///   <item><description><code>[Dropdown("ValuesReferences"]</code></description></item>
    ///   <item><description><code>[Dropdown("Choice A", "Choice B", "Choice C")]</code></description></item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DropdownAttribute : PropertyAttribute
    {
        public readonly bool isReference;
        public readonly string Path;
        public readonly object[] objects;

        /// <summary>
        /// Prend le chemin en paramètre
        /// </summary>
        /// <param name="path"></param>
        public DropdownAttribute(string path)
        {
            isReference = true;
            Path = path;
        }

        /// <summary>
        /// Prend les valeurs en paramètre
        /// </summary>
        /// <param name="objects"></param>
        public DropdownAttribute(params object[] objects)
        {
            isReference= false;
            this.objects = objects;
        }
    }
}