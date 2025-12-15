using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomPropertyDrawer(typeof(MVsPool<>))]
    public class MVsPoolDrawer : PropertyDrawer
    {
        private GUIStyle _helpBoxNoTopMargin;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lineH = EditorGUIUtility.singleLineHeight;
            float vsp = EditorGUIUtility.standardVerticalSpacing;
            float y = position.y;

            // Foldout header
            var headerRect = new Rect(position.x, y, position.width, lineH);
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);
            y += lineH + vsp;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                SerializedProperty prefabProp = property.FindPropertyRelative("prefab");
                SerializedProperty setParentProp = property.FindPropertyRelative("m_SetParent");
                SerializedProperty parentProp = property.FindPropertyRelative("parent");
                SerializedProperty limitSizeProp = property.FindPropertyRelative("m_LimitSize");
                SerializedProperty maxSizeProp = property.FindPropertyRelative("MaximumPoolSize");
                SerializedProperty prewarmProp = property.FindPropertyRelative("m_Prewarm");
                SerializedProperty prewarmCountProp = property.FindPropertyRelative("PrewarmCount");

                EditorGUILayout.BeginVertical(GetHelpBoxStyle());
                if (prefabProp != null)
                {
                    Rect rowRect = new Rect(position.x - 16, y, position.width, lineH);
                    rowRect = EditorGUI.IndentedRect(rowRect);
                    EditorGUI.LabelField(new Rect(rowRect.x, rowRect.y, 96, lineH), "Prefab");

                    Rect fieldRect = new Rect(rowRect.x + 128, rowRect.y, rowRect.width - 112, lineH);
                    EditorGUI.PropertyField(fieldRect, prefabProp, GUIContent.none);

                    y += lineH + vsp + 5;
                }

                DrawToggleAndFieldSameLine("Set Parent", setParentProp, parentProp);
                DrawToggleAndFieldSameLine("Limit Count", limitSizeProp, maxSizeProp);
                DrawToggleAndFieldSameLine("Prewarm", prewarmProp, prewarmCountProp);

                EditorGUI.indentLevel--;

                void DrawToggleAndFieldSameLine(string label, SerializedProperty toggleProp, SerializedProperty fieldProp)
                {
                    Rect rowRect = new Rect(position.x - 16, y, position.width, lineH);
                    rowRect = EditorGUI.IndentedRect(rowRect);

                    EditorGUI.LabelField(new Rect(rowRect.x, rowRect.y, 96, lineH), label);

                    if (toggleProp == null) return;

                    Rect toggleRect = new Rect(rowRect.x + 96, rowRect.y, 16, lineH);
                    bool newVal = EditorGUI.Toggle(toggleRect, GUIContent.none, toggleProp.boolValue);
                    if (newVal != toggleProp.boolValue)
                        toggleProp.boolValue = newVal;

                    if (toggleProp == null || !toggleProp.boolValue || fieldProp == null)
                        GUI.enabled = false;
                    
                    Rect fieldRect = new Rect(rowRect.x + 128, rowRect.y, rowRect.width - 112, lineH);
                    EditorGUI.PropertyField(fieldRect, fieldProp, GUIContent.none);
                    GUI.enabled = true;

                    y += lineH + vsp;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            float vsp = EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                var prefabProp = property.FindPropertyRelative("prefab");

                if (prefabProp != null)
                    height += EditorGUI.GetPropertyHeight(prefabProp, true) + vsp;

                height += (EditorGUIUtility.singleLineHeight + vsp) * 3f;
            }

            return height;
        }

        private GUIStyle GetHelpBoxStyle()
        {
            if (_helpBoxNoTopMargin == null)
            {
                _helpBoxNoTopMargin = new GUIStyle(EditorStyles.helpBox);
                var m = _helpBoxNoTopMargin.margin;
                _helpBoxNoTopMargin.margin = new RectOffset(m.left, m.right, 0, m.bottom);
            }
            return _helpBoxNoTopMargin;
        }
    }
}