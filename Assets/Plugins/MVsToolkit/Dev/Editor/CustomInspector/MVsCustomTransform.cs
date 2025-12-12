using MVsToolkit.Utils;
using System;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BetterInterface
{
    [CustomEditor(typeof(Transform), true), CanEditMultipleObjects]
    public class MVsCustomTransform : Editor
    {
        const float labelWidth = 95f;
        const float resetSize = 14f;
        const float spacing = 4f;
        const float spaceBetweenLabelAndField = 30;
        const float spaceBetweenLinkIconAndField = 22f;

        bool isPositionExpanded = false;
        bool isRotationExpanded = false;
        bool isScaleExpanded = false;

        private GUIStyle IconButtonStyle
        {
            get
            {
                GUIStyle s;
                s = new GUIStyle();
                s.padding = new RectOffset(0, 0, 0, 0);
                s.margin = new RectOffset(0, 0, 0, 0);
                s.border = new RectOffset(0, 0, 0, 0);
                s.alignment = TextAnchor.MiddleCenter;
                s.imagePosition = ImagePosition.ImageOnly;
                s.normal.background = null;
                s.hover.background = null;
                s.active.background = null;
                s.focused.background = null;

                return s;
            }
        }

        public override void OnInspectorGUI()
        {
            Transform firstTransform = (Transform)target;

            EditorGUILayout.Space(2);
            DrawPosition(firstTransform);
            DrawRotation(firstTransform);
            DrawScale(firstTransform);
            EditorGUILayout.Space(2);
        }

        void DrawPosition(Transform t)
        {
            EditorGUIUtility.labelWidth = labelWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            Rect foldRect = new Rect(rect.x + 16, rect.y, 14f, rect.height);
            isPositionExpanded = EditorGUI.Foldout(foldRect, isPositionExpanded, GUIContent.none, true);

            Rect labelRect = new Rect(rect.x + 14f + spacing, rect.y, labelWidth - 14f - spacing, rect.height);
            EditorGUI.LabelField(labelRect, "Position");

            float fieldWidth = rect.width - (labelWidth + resetSize + spacing * 4 + spaceBetweenLabelAndField);
            Rect fieldRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField, rect.y, fieldWidth, rect.height);
            t.localPosition = EditorGUI.Vector3Field(fieldRect, GUIContent.none, t.localPosition);

            Rect resetRect = new Rect(fieldRect.xMax + spacing, rect.y + (rect.height - resetSize) / 2f, resetSize, resetSize);
            if (GUI.Button(resetRect, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                t.localPosition = Vector3.zero;

            if (isPositionExpanded)
            {
                Rect rect2 = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                // Label parfaitement aligné
                Rect labelRect2 = new Rect(rect2.x + 14f + spacing, rect2.y, labelWidth - 14f - spacing, rect2.height);
                GUIStyle rich = new GUIStyle(EditorStyles.label);
                rich.richText = true;

                EditorGUI.LabelField(labelRect2, "<color=grey>World</color>", rich);
                // Champ Vector3 aligné
                Rect fieldRect2 = new Rect(rect2.x + labelWidth + spaceBetweenLabelAndField, rect2.y, fieldWidth, rect2.height);
                t.position = EditorGUI.Vector3Field(fieldRect2, GUIContent.none, t.position);

                // Reset aligné
                Rect resetRect2 = new Rect(fieldRect2.xMax + spacing, rect2.y + (rect2.height - resetSize) / 2f, resetSize, resetSize);
                if (GUI.Button(resetRect2, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                    t.position = Vector3.zero;

                EditorGUILayout.Space(1);
            }
        }

        void DrawRotation(Transform t)
        {
            EditorGUIUtility.labelWidth = labelWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            Rect foldRect = new Rect(rect.x + 16, rect.y, 14f, rect.height);
            isRotationExpanded = EditorGUI.Foldout(foldRect, isRotationExpanded, GUIContent.none, true);

            Rect labelRect = new Rect(rect.x + 14f + spacing, rect.y, labelWidth - 14f - spacing, rect.height);
            EditorGUI.LabelField(labelRect, "Rotation");

            float fieldWidth = rect.width - (labelWidth + resetSize + spacing * 4 + spaceBetweenLabelAndField);
            Rect fieldRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField, rect.y, fieldWidth, rect.height);
            t.localRotation = Quaternion.Euler(EditorGUI.Vector3Field(fieldRect, GUIContent.none, t.localEulerAngles));

            Rect resetRect = new Rect(fieldRect.xMax + spacing, rect.y + (rect.height - resetSize) / 2f, resetSize, resetSize);
            if (GUI.Button(resetRect, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                t.localRotation = Quaternion.identity;

            if (isRotationExpanded)
            {
                Rect rect2 = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                // Label parfaitement aligné
                Rect labelRect2 = new Rect(rect2.x + 14f + spacing, rect2.y, labelWidth - 14f - spacing, rect2.height);
                GUIStyle rich = new GUIStyle(EditorStyles.label);
                rich.richText = true;

                EditorGUI.LabelField(labelRect2, "<color=grey>World</color>", rich);
                // Champ Vector3 aligné
                Rect fieldRect2 = new Rect(rect2.x + labelWidth + spaceBetweenLabelAndField, rect2.y, fieldWidth, rect2.height);
                t.rotation = Quaternion.Euler(EditorGUI.Vector3Field(fieldRect2, GUIContent.none, t.eulerAngles));

                // Reset aligné
                Rect resetRect2 = new Rect(fieldRect2.xMax + spacing, rect2.y + (rect2.height - resetSize) / 2f, resetSize, resetSize);
                if (GUI.Button(resetRect2, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                    t.eulerAngles = Vector3.zero;

                EditorGUILayout.Space(1);
            }
        }
        
        void DrawScale(Transform t)
        {
            EditorGUIUtility.labelWidth = labelWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            Rect foldRect = new Rect(rect.x + 16, rect.y, 14f, rect.height);
            isScaleExpanded = EditorGUI.Foldout(foldRect, isScaleExpanded, GUIContent.none, true);

            Rect labelRect = new Rect(rect.x + 14f + spacing, rect.y, labelWidth - 14f - spacing, rect.height);
            EditorGUI.LabelField(labelRect, "Scale");

            Rect linkButtonRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField - spaceBetweenLinkIconAndField, rect.y + (rect.height - 14f) / 2f - 2, 16f, 16f);
            if (GUI.Button(linkButtonRect, EditorGUIUtility.IconContent("d_Unlinked"), IconButtonStyle))
                Debug.Log("dd");

                float fieldWidth = rect.width - (labelWidth + resetSize + spacing * 4 + spaceBetweenLabelAndField);
            Rect fieldRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField, rect.y, fieldWidth, rect.height);
            t.localScale = EditorGUI.Vector3Field(fieldRect, GUIContent.none, t.localScale);

            Rect resetRect = new Rect(fieldRect.xMax + spacing, rect.y + (rect.height - resetSize) / 2f, resetSize, resetSize);
            if (GUI.Button(resetRect, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                t.localScale = Vector3.one;

            if (isScaleExpanded)
            {
                Rect rect2 = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                // Label parfaitement aligné
                Rect labelRect2 = new Rect(rect2.x + 14f + spacing, rect2.y, labelWidth - 14f - spacing, rect2.height);
                GUIStyle rich = new GUIStyle(EditorStyles.label);
                rich.richText = true;

                EditorGUI.LabelField(labelRect2, "<color=grey>World</color>", rich);
                // Champ Vector3 aligné
                Rect fieldRect2 = new Rect(rect2.x + labelWidth + spaceBetweenLabelAndField, rect2.y, fieldWidth, rect2.height);
                
                Vector3 worldScale = t.lossyScale;
                Vector3 newWorldScale = EditorGUI.Vector3Field(fieldRect2, GUIContent.none, worldScale);
                if (newWorldScale != worldScale)
                    SetWorldScale(t, newWorldScale);

                // Reset aligné
                Rect resetRect2 = new Rect(fieldRect2.xMax + spacing, rect2.y + (rect2.height - resetSize) / 2f, resetSize, resetSize);
                if (GUI.Button(resetRect2, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                    SetWorldScale(t, Vector3.one);

                EditorGUILayout.Space(1);
            }
        }

        void SetWorldScale(Transform t, Vector3 worldScale)
        {
            if (t.parent == null)
            {
                t.localScale = worldScale;
            }
            else
            {
                Vector3 parentScale = t.parent.lossyScale;

                t.localScale = new Vector3(
                    worldScale.x / parentScale.x,
                    worldScale.y / parentScale.y,
                    worldScale.z / parentScale.z
                );
            }
        }
    }
}