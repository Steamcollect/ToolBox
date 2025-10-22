namespace ToolBox.Dev
{
    using UnityEditor;
    using UnityEngine;
    using System.Reflection;

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIf_PropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool ShouldShow(SerializedProperty property)
        {
            ShowIfAttribute attr = (ShowIfAttribute)attribute;
            Object target = property.serializedObject.targetObject;
            System.Type type = target.GetType();

            FieldInfo conditionField = type.GetField(attr.ConditionField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (conditionField == null)
            {
                Debug.LogWarning($"[ShowIf] Champ '{attr.ConditionField}' introuvable sur {type.Name}");
                return true; // ne cache pas pour �viter de masquer par erreur
            }

            object conditionValue = conditionField.GetValue(target);

            // G�re les bools
            if (conditionValue is bool boolVal)
            {
                if (attr.CompareValue is bool boolCompare)
                    return boolVal == boolCompare;

                Debug.LogWarning($"[ShowIf] Mauvais type de comparaison : '{attr.ConditionField}' est bool, mais CompareValue n'est pas bool");
                return true;
            }

            // G�re les enums
            if (conditionValue != null && conditionValue.GetType().IsEnum)
            {
                try
                {
                    object compareEnum = System.Enum.Parse(conditionValue.GetType(), attr.CompareValue.ToString());
                    return conditionValue.Equals(compareEnum);
                }
                catch
                {
                    Debug.LogWarning($"[ShowIf] Impossible de comparer enum '{conditionValue}' avec '{attr.CompareValue}'");
                    return true;
                }
            }

            // Autres types non pris en charge
            Debug.LogWarning($"[ShowIf] Type non support� : '{conditionField.Name}' doit �tre bool ou enum");
            return true;
        }
    }
}