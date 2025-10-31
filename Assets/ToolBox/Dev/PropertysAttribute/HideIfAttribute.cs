using UnityEngine;

namespace ToolBox.Dev
{
    /// <summary>
    /// Cache la propriété dans l’inspecteur si la variable spécifiée correspond à la valeur donnée.
    ///
    /// <para>Fonctionne uniquement avec :</para>
    /// <list type="bullet">
    ///   <item><description>bool → ex: [HideIf("isEnabled", true)]</description></item>
    ///   <item><description>enum → ex: [HideIf("mode", GameMode.Debug)]</description></item>
    /// </list>
    ///
    /// <para>
    /// Si la condition n’est pas remplie, la propriété est affichée normalement.
    /// </para>
    /// 
    /// <para>
    /// Peux être utlilisé plusieurs fois sur la même variable
    /// </para>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string ConditionField;
        public readonly object CompareValue;

        /// <param name="conditionField">Nom du champ bool ou enum à tester.</param>
        /// <param name="compareValue">Valeur de comparaison. Si le champ est égal à cette valeur, la propriété est cachée.</param>
        public HideIfAttribute(string conditionField, object compareValue)
        {
            ConditionField = conditionField;
            CompareValue = compareValue;
        }
    }
}