namespace MVsToolkit.Dev
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(InterfaceReference<>), true)]
    public class InterfaceReference_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty objProp = property.FindPropertyRelative("_object");

            EditorGUI.BeginProperty(position, label, property);
            UnityEngine.Object assignedObj = EditorGUI.ObjectField(position, label, objProp.objectReferenceValue, typeof(UnityEngine.Object), true);

            if (assignedObj != objProp.objectReferenceValue)
            {
                if (assignedObj == null)
                {
                    objProp.objectReferenceValue = null;
                }
                else
                {
                    // Safely resolve the target generic type argument (T)
                    Type targetType = null;
                    if (fieldInfo != null)
                    {
                        var args = fieldInfo.FieldType.IsGenericType ? fieldInfo.FieldType.GetGenericArguments() : Array.Empty<Type>();
                        if (args.Length > 0)
                            targetType = args[0];
                    }

                    if (targetType != null && targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        // IEnumerable<> support: store GameObject or its GameObject when component assigned
                        if (assignedObj is GameObject go)
                        {
                            objProp.objectReferenceValue = go;
                        }
                        else if (assignedObj is Component comp)
                        {
                            objProp.objectReferenceValue = comp.gameObject;
                        }
                        else if (targetType.IsAssignableFrom(assignedObj.GetType()))
                        {
                            objProp.objectReferenceValue = assignedObj;
                        }
                    }
                    else if (targetType != null)
                    {
                        if (assignedObj is GameObject go)
                        {
                            // Autoriser si le GameObject a un composant qui implémente l’interface
                            var comp = go.GetComponent(targetType);
                            if (comp != null)
                                objProp.objectReferenceValue = comp as UnityEngine.Object;
                        }
                        else if (targetType.IsAssignableFrom(assignedObj.GetType()))
                        {
                            objProp.objectReferenceValue = assignedObj;
                        }
                        else
                        {
                            // Fallback: if target type known but assignment invalid, clear
                            objProp.objectReferenceValue = null;
                        }
                    }
                    else
                    {
                        // When we cannot resolve generic argument (e.g., in lists), be permissive:
                        // Store GameObject or Component directly; InterfaceReference.Value will attempt casting.
                        if (assignedObj is Component c)
                            objProp.objectReferenceValue = c;
                        else
                            objProp.objectReferenceValue = assignedObj;
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}