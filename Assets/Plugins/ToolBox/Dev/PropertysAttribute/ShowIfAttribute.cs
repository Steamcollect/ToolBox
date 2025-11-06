using UnityEngine;

namespace ToolBox.Dev
{
    /// <summary>
    /// Affiche la propriété dans l’inspecteur **uniquement si**
    /// une autre variable (bool ou enum) correspond à la valeur indiquée.
    ///
    /// <para>Fonctionne uniquement avec :</para>
    /// <list type="bullet">
    ///   <item><description>bool → ex: <code>[ShowIf("isEnabled", true)]</code></description></item>
    ///   <item><description>enum → ex: <code>[ShowIf("mode", GameMode.Debug)]</code></description></item>
    /// </list>
    ///
    /// <para>
    /// Si la condition n’est pas remplie, la propriété est cachée.
    /// </para>
    ///
    /// <para>
    /// Peux être utlilisé plusieurs fois sur la même variable
    /// </para>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string ConditionField;
        public readonly object CompareValue;

        /// <param name="conditionField">
        /// Nom du champ bool ou enum à vérifier.
        /// </param>
        /// <param name="compareValue">
        /// Valeur de comparaison.  
        /// Si la variable ciblée est égale à cette valeur, la propriété sera affichée.
        /// </param>
        public ShowIfAttribute(string conditionField, object compareValue)
        {
            ConditionField = conditionField;
            CompareValue = compareValue;
        }
    }
}