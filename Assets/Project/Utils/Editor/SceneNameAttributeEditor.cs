using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] with a string.");
                return;
            }
            
            List<string> scenes = new List<string>{Capacity = EditorBuildSettings.scenes.Length};
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (!EditorBuildSettings.scenes[i].enabled) continue;
                scenes.Add(System.IO.Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path));
            }

            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(scenes.ToArray(), property.stringValue));
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, scenes.ToArray());

            if (selectedIndex >= 0 && selectedIndex < scenes.Count)
            {
                property.stringValue = scenes[selectedIndex];
            }
        }
    }