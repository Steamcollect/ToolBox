namespace ToolBox.Dev
{
    using UnityEditor;
    using UnityEngine;
    using System.Reflection;

    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIf_PropertyDrawer : PropertyDrawer
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
            HideIfAttribute attr = (HideIfAttribute)attribute;
            Object target = property.serializedObject.targetObject;
            System.Type type = target.GetType();

            FieldInfo conditionField = type.GetField(attr.ConditionField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (conditionField == null)
            {
                Debug.LogWarning($"[HideIf] Champ '{attr.ConditionField}' introuvable sur {type.Name}");
                return true; // on affiche quand m�me pour �viter les erreurs
            }

            object conditionValue = conditionField.GetValue(target);

            // Gestion bool
            if (conditionValue is bool boolVal)
            {
                if (attr.CompareValue is bool boolCompare)
                    return boolVal != boolCompare; // cache si �gaux
                Debug.LogWarning($"[HideIf] Mauvais type de comparaison : '{attr.ConditionField}' est bool, mais CompareValue n'est pas bool");
                return true;
            }

            // Gestion enum
            if (conditionValue != null && conditionValue.GetType().IsEnum)
            {
                try
                {
                    object compareEnum = System.Enum.Parse(conditionValue.GetType(), attr.CompareValue.ToString());
                    return !conditionValue.Equals(compareEnum); // cache si �gaux
                }
                catch
                {
                    Debug.LogWarning($"[HideIf] Impossible de comparer enum '{conditionValue}' avec '{attr.CompareValue}'");
                    return true;
                }
            }

            Debug.LogWarning($"[HideIf] Type non support� : '{conditionField.Name}' doit �tre bool ou enum");
            return true;
        }
    }
}