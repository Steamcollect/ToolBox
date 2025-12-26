using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRange_PropertyDrawer : PropertyDrawer
    {
        const float minLabelWidth = 122f;
        float labelWidth;
        const float fieldWidth = 40;
        const float spacing = 4;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxRangeAttribute range = (MinMaxRangeAttribute)attribute;

            if (position.width * .3f > minLabelWidth)
            {
                labelWidth = position.width * .3f;
            }
            else
            {
                labelWidth = minLabelWidth;
            }

            Rect variableLabelR = new Rect(position.x, position.y, labelWidth, position.height);
            Rect sliderR = new Rect(position.x + labelWidth + fieldWidth + spacing, position.y, position.width - labelWidth - (fieldWidth * 2) - (spacing * 2), position.height);
            Rect lFieldR = new Rect(sliderR.x - fieldWidth - spacing, position.y, fieldWidth, position.height);
            Rect rFieldR = new Rect(sliderR.x + sliderR.width + spacing, position.y, fieldWidth, position.height);

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                EditorGUI.BeginProperty(position, label, property);
                Vector2 value = property.vector2Value;

                GUI.Label(variableLabelR, label);

                value.x = EditorGUI.FloatField(lFieldR, property.vector2Value.x);
                value.y = EditorGUI.FloatField(rFieldR, property.vector2Value.y);

                EditorGUI.MinMaxSlider(sliderR, GUIContent.none, ref value.x, ref value.y, range.FMin, range.FMax);
                value.x = Mathf.Clamp(value.x, range.FMin, value.y);
                value.y = Mathf.Clamp(value.y, value.x, range.FMax);
                property.vector2Value = value;
                EditorGUI.EndProperty();
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                EditorGUI.BeginProperty(position, label, property);
                Vector2Int value = property.vector2IntValue;

                GUI.Label(variableLabelR, label);

                value.x = EditorGUI.IntField(lFieldR, property.vector2IntValue.x);
                value.y = EditorGUI.IntField(rFieldR, property.vector2IntValue.y);

                float fx = value.x;
                float fy = value.y;
                EditorGUI.MinMaxSlider(sliderR, GUIContent.none, ref fx, ref fy, range.IMin, range.IMax);

                int newX = Mathf.Clamp(Mathf.RoundToInt(fx), range.IMin, value.y);
                int newY = Mathf.Clamp(Mathf.RoundToInt(fy), newX, range.IMax);
                value.x = newX;
                value.y = newY;

                property.vector2IntValue = value;
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use MinMaxRange with Vector2 or Vector2Int.");
            }
        }
    }
}