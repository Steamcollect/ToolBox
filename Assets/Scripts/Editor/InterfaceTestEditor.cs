using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InterfaceTest))]
public class InterfaceTestEditor : Editor
{
    private SerializedProperty iterator;

    private void OnEnable()
    {
        iterator = serializedObject.GetIterator();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = iterator.Copy();
        bool enterChildren = true;

        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;

            FieldInfo field = serializedObject.targetObject.GetType().GetField(
                property.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (field == null)
            {
                EditorGUILayout.PropertyField(property, true);
                continue;
            }

            SerializedInterfaceAttribute serializedInterface =
                field.GetCustomAttribute<SerializedInterfaceAttribute>();

            if (serializedInterface != null)
            {
                DrawInterfaceField(property, field);
            }
            else
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawInterfaceField(SerializedProperty property, FieldInfo field)
    {
        Type interfaceType = field.FieldType;
        UnityEngine.Object oldRef = property.objectReferenceValue;
        UnityEngine.Object newRef = EditorGUILayout.ObjectField(
            ObjectNames.NicifyVariableName(property.name),
            oldRef,
            typeof(UnityEngine.Object),
            true
        );

        if (newRef != oldRef)
        {
            if (newRef == null || interfaceType.IsAssignableFrom(newRef.GetType()))
            {
                property.objectReferenceValue = newRef;
            }
            else if (newRef is GameObject go)
            {
                Component component = go.GetComponent(interfaceType);
                if (component != null)
                {
                    property.objectReferenceValue = component;
                }
                else
                {
                    Debug.LogWarning($"L'objet {go.name} n'implémente pas {interfaceType.Name}");
                }
            }
            else
            {
                Debug.LogWarning($"{newRef.name} n'implémente pas {interfaceType.Name}");
            }
        }
    }
}