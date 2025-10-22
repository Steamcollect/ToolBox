namespace ToolBox.Dev
{
    using UnityEngine;

    /// <summary>
    /// Affiche un dropdown dans l'inspector avec les valeurs donnée
    /// 
    /// <para>
    /// La référence doit être une string avec le nom de la variable connecté.
    /// </para>
    /// 
    /// <para>
    /// La variable peut venir d'un script référencé
    /// </para>
    ///  
    /// <para>Fonctionne uniquement avec :</para>
    /// <list type="bullet">
    ///   <item><description>enum → ex: <code>[StringDropdown("Values"]</code></description></item>
    ///   <item><description>bool → ex: <code>[StringDropdown("myReferences.Values")]</code></description></item>
    /// </list>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class StringDropdownAttribute : PropertyAttribute
    {
        public readonly string Path;

        public StringDropdownAttribute(string path)
        {
            Path = path;
        }
    }
}