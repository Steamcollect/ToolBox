namespace ToolBox.Dev
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(InterfaceReference<>), true)]
    public class InterfaceReference_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty objProp = property.FindPropertyRelative("_object");

            EditorGUI.BeginProperty(position, label, property);
            Object assignedObj = EditorGUI.ObjectField(position, label, objProp.objectReferenceValue, typeof(Object), true);

            if (assignedObj != objProp.objectReferenceValue)
            {
                if (assignedObj == null)
                {
                    objProp.objectReferenceValue = null;
                }
                else
                {
                    var targetType = fieldInfo.FieldType.GetGenericArguments()[0];

                    if (assignedObj is GameObject go)
                    {
                        // Autoriser si le GameObject a un composant qui implémente l’interface
                        var comp = go.GetComponent(targetType);
                        if (comp != null)
                            objProp.objectReferenceValue = comp as Object;
                    }
                    else if (targetType.IsAssignableFrom(assignedObj.GetType()))
                    {
                        objProp.objectReferenceValue = assignedObj;
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}