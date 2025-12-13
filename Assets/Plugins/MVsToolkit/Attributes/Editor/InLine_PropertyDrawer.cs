using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InLine_PropertyDrawer : PropertyDrawer
    {
        private const float Padding = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int originalIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var children = GetChildren(property);
            if (children.Count == 0)
            {
                EditorGUI.LabelField(position, "Empty struct");
                EditorGUI.indentLevel = originalIndent;
                EditorGUI.EndProperty();
                return;
            }

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            float x = position.x;
            float y = position.y;
            float maxWidth = position.width;
            float remainingWidth = maxWidth;

            foreach (var c in children)
            {
                float fieldWidth = GetPropertyWidth(c);

                // Si le champ ne tient pas sur la ligne, retour à la ligne suivante
                if (fieldWidth > remainingWidth)
                {
                    y += lineHeight + spacing;
                    x = position.x;
                    remainingWidth = maxWidth;
                }

                Rect fieldRect = new Rect(x, y, fieldWidth, lineHeight);
                EditorGUI.PropertyField(fieldRect, c, GUIContent.none);

                x += fieldWidth + Padding;
                remainingWidth -= fieldWidth + Padding;
            }

            EditorGUI.indentLevel = originalIndent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var children = GetChildren(property);
            if (children.Count == 0)
                return EditorGUIUtility.singleLineHeight;

            float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float maxWidth = EditorGUIUtility.labelWidth > 1 ? EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 40 : 250;
            float usedWidth = 0f;
            int lineCount = 1;

            foreach (var c in children)
            {
                float width = GetPropertyWidth(c);
                if (usedWidth + width > maxWidth)
                {
                    lineCount++;
                    usedWidth = 0;
                }
                usedWidth += width + Padding;
            }

            // +2 pour la marge dans les ReorderableList
            return lineCount * lineHeight + 2f;
        }

        private List<SerializedProperty> GetChildren(SerializedProperty property)
        {
            var children = new List<SerializedProperty>();
            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            bool enterChildren = iterator.NextVisible(true);
            if (!enterChildren) return children;

            do
            {
                if (SerializedProperty.EqualContents(iterator, end))
                    break;
                children.Add(iterator.Copy());
            } while (iterator.NextVisible(false));

            return children;
        }

        private float GetPropertyWidth(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean: return 18f;
                case SerializedPropertyType.Integer: return 60f;
                case SerializedPropertyType.Float: return 60f;
                case SerializedPropertyType.Enum: return 80f;
                case SerializedPropertyType.Vector2: return 120f;
                case SerializedPropertyType.Vector3: return 160f;
                case SerializedPropertyType.Color: return 100f;
                case SerializedPropertyType.String: return 100f;
                default: return 80f;
            }
        }
    }
}