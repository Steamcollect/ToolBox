using UnityEngine;
using UnityEditor;

namespace MVsToolkit.Dev
{
    [CustomPropertyDrawer(typeof(TagNameAttribute))]
    public class TagName_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                // Récupération de tous les tags existants
                string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

                int index = Mathf.Max(0, System.Array.IndexOf(tags, property.stringValue));

                EditorGUI.BeginProperty(position, label, property);

                int newIndex = EditorGUI.Popup(position, label.text, index, tags);

                if (newIndex >= 0 && newIndex < tags.Length)
                {
                    property.stringValue = tags[newIndex];
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [TagName] with strings only.");
            }
        }
    }
}