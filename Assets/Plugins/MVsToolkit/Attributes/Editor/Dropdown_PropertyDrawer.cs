namespace MVsToolkit.Dev
{
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class Dropdown_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedPropertyType type = property.propertyType;
            
            // Check le type de la variable target
            if(type != SerializedPropertyType.Integer 
                && type != SerializedPropertyType.String
                && type != SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(position, "Error : Dropdown only supports string, int or float fields");
                return;
            }

            object target = property.serializedObject.targetObject;
            DropdownAttribute dropdown = attribute as DropdownAttribute;

            if (type == SerializedPropertyType.String)
            {
                string[] options = dropdown.isReference ?
                    ResolveArrayFromReference<string>(target, dropdown.Path)
                    : Array.ConvertAll(dropdown.objects, o => o.ToString());

                HandleDropdown(options, SerializedPropertyType.String, position, property, label);
            }
            else if (type == SerializedPropertyType.Float)
            {
                float[] options = dropdown.isReference ?
                    ResolveArrayFromReference<float>(target, dropdown.Path)
                    : Array.ConvertAll(dropdown.objects, o => Convert.ToSingle(o));
                HandleDropdown(options, SerializedPropertyType.Float, position, property, label);
            }
            else if (type == SerializedPropertyType.Integer)
            {
                int[] options = dropdown.isReference ?
                    ResolveArrayFromReference<int>(target, dropdown.Path)
                    : Array.ConvertAll(dropdown.objects, o => Convert.ToInt32(o));
                HandleDropdown(options, SerializedPropertyType.Integer, position, property, label);
            }
        }

        void HandleDropdown<T>(T[] options, SerializedPropertyType type,
            Rect position, SerializedProperty property, GUIContent label)
        {
            string[] optionsLabel = ConvertToStringArray(options);

            EditorGUI.BeginProperty(position, label, property);

            if (optionsLabel == null || optionsLabel.Length == 0)
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUI.PropertyField(position, property, label);
                return;
            }

            // Déterminer la valeur actuelle selon le type
            string currentValue = type switch
            {
                SerializedPropertyType.Float => property.floatValue.ToString(),
                SerializedPropertyType.Integer => property.intValue.ToString(),
                SerializedPropertyType.String => property.stringValue,
                _ => string.Empty
            };

            int currentIndex = Mathf.Max(0, Array.IndexOf(optionsLabel, currentValue));
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, optionsLabel);

            if (newIndex != currentIndex)
            {
                switch (type)
                {
                    case SerializedPropertyType.Float:
                        property.floatValue = (float)Convert.ChangeType(options[newIndex], typeof(float));
                        break;
                    case SerializedPropertyType.Integer:
                        property.intValue = (int)Convert.ChangeType(options[newIndex], typeof(int));
                        break;
                    case SerializedPropertyType.String:
                        property.stringValue = options[newIndex].ToString();
                        break;
                }
            }
        }

        private T[] ResolveArrayFromReference<T>(object target, string path)
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

                // Recherche récursive dans la hiérarchie
                FieldInfo field = GetFieldRecursive(t, part);
                PropertyInfo prop = GetPropertyRecursive(t, part);

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

            if (current is T[] array)
                return array;

            if (current is List<T> list)
                return list.ToArray();

            return null;
        }

        private FieldInfo GetFieldRecursive(Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    return field;
                type = type.BaseType;
            }
            return null;
        }

        private PropertyInfo GetPropertyRecursive(Type type, string propertyName)
        {
            while (type != null)
            {
                PropertyInfo prop = type.GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null)
                    return prop;
                type = type.BaseType;
            }
            return null;
        }

        string[] ConvertToStringArray<T>(T[] array)
        {
            if (array == null) return Array.Empty<string>();

            string[] result = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
    }
}