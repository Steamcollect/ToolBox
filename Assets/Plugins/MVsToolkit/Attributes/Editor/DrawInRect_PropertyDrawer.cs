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
            var attr = (DrawInRectAttribute)attribute;
            return attr.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (DrawInRectAttribute)attribute;

            // Récupère l'objet cible
            Object target = property.serializedObject.targetObject;
            MethodInfo method = target.GetType().GetMethod(
                attr.methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (method != null)
            {
                method.Invoke(target, new object[] { position });
            }
            else
            {
                EditorGUI.HelpBox(position, $"Méthode '{attr.methodName}' introuvable", MessageType.Error);
            }
        }
    }

}