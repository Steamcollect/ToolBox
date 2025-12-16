using System.Reflection;
using MVsToolkit.Dev;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomPropertyDrawer(typeof(DrawInRectAttribute))]
    public class DrawInRect_PropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ((DrawInRectAttribute)attribute).height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (DrawInRectAttribute)attribute;
            object target = property.serializedObject.targetObject;

            MethodInfo method = FindMethodInHierarchy(
                target.GetType(),
                attr.methodName
            );

            if (method != null)
            {
                method.Invoke(target, new object[] { position });
            }
            else
            {
                EditorGUI.HelpBox(
                    position,
                    $"Méthode '{attr.methodName}(Rect)' introuvable dans la hiérarchie",
                    MessageType.Error
                );
            }
        }

        private MethodInfo FindMethodInHierarchy(System.Type type, string methodName)
        {
            while (type != null)
            {
                MethodInfo method = type.GetMethod(
                    methodName,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly
                );

                if (method != null)
                    return method;

                type = type.BaseType;
            }

            return null;
        }
    }

}