namespace MVsToolkit.Dev
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnly_PropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Préserve la hauteur normale (y compris les enfants/arrays)
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.DisabledScope(true)) // grise et bloque l’édition
            {
                EditorGUI.PropertyField(position, property, label, includeChildren: true);
            }
        }
    }

}