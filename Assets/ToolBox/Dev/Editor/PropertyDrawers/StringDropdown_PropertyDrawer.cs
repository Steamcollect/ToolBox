namespace ToolBox.Dev
{
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    [CustomPropertyDrawer(typeof(StringDropdownAttribute))]
    public class StringDropdown_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Error: StringDropdown only supports string fields.");
                return;
            }

            var dropdown = attribute as StringDropdownAttribute;
            object target = property.serializedObject.targetObject;

            string[] options = ResolveStringArray(target, dropdown.Path);

            if (options == null || options.Length == 0)
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUI.PropertyField(position, property, label);
                return;
            }

            string currentValue = property.stringValue;
            int currentIndex = Mathf.Max(0, Array.IndexOf(options, currentValue));
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, options);

            if (newIndex != currentIndex)
                property.stringValue = options[newIndex];
        }

        private string[] ResolveStringArray(object target, string path)
        {
            if (target == null || string.IsNullOrEmpty(path))
                return null;

            string[] parts = path.Split('.');
            object current = target;

            foreach (string part in parts)
            {
                if (current == null)
                    return null;

                Type t = current.GetType();
                FieldInfo field = t.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                PropertyInfo prop = t.GetProperty(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // Récupère la valeur
                if (field != null)
                    current = field.GetValue(current);
                else if (prop != null)
                    current = prop.GetValue(current);
                else
                    return null;

                // Si c’est une référence UnityEngine.Object, on bascule sur sa "vraie" instance
                if (current is UnityEngine.Object unityObj && !(current is GameObject))
                {
                    current = unityObj;
                }
            }

            // Support des string[] et List<string>
            if (current is string[] array)
                return array;

            if (current is List<string> list)
                return list.ToArray();

            return null;
        }
    }
}