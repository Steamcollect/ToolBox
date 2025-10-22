namespace ToolBox.Dev
{
    using UnityEditor;
    using UnityEngine;
    using System.Reflection;

    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class Button_PropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;

            float height = EditorGUIUtility.singleLineHeight + 4; // Button size
            if (buttonAttribute.ShowVariable)
            {
                height += EditorGUI.GetPropertyHeight(property, label, true);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;

            Rect currentRect = position;

            // Draw or not the variable
            if (buttonAttribute.ShowVariable)
            {
                float propHeight = EditorGUI.GetPropertyHeight(property, label, true);
                Rect propertyRect = new Rect(currentRect.x, currentRect.y, currentRect.width, propHeight);
                EditorGUI.PropertyField(propertyRect, property, label, true);

                currentRect.y += propHeight + 2;
            }

            // Button
            Rect buttonRect = new Rect(currentRect.x, currentRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);
            string buttonLabel = string.IsNullOrEmpty(buttonAttribute.Path) ? "Execute Action" : buttonAttribute.Path;

            if (GUI.Button(buttonRect, buttonLabel))
            {
                Object target = property.serializedObject.targetObject;
                System.Type type = target.GetType();

                MethodInfo method = type.GetMethod(buttonAttribute.Path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method != null)
                {
                    method.Invoke(target, null);
                }
                else
                {
                    Debug.LogWarning($"Méthode '{buttonAttribute.Path}' introuvable sur {type.Name}");
                }
            }
        }
    }
}