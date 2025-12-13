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
        bool isScaleLinked = true;

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

        void DrawPosition(Transform first)
        {
            EditorGUIUtility.labelWidth = labelWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            Rect foldRect = new Rect(rect.x + 16, rect.y, 14f, rect.height);
            isPositionExpanded = EditorGUI.Foldout(foldRect, isPositionExpanded, GUIContent.none, true);

            Rect labelRect = new Rect(rect.x + 14f + spacing, rect.y, labelWidth - 14f - spacing, rect.height);
            EditorGUI.LabelField(labelRect, "Position");

            float fieldWidth = rect.width - (labelWidth + resetSize + spacing * 4 + spaceBetweenLabelAndField);
            Rect fieldRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField, rect.y, fieldWidth, rect.height);

            Vector3 prevLocal = first.localPosition;
            Vector3 editedLocal = EditorGUI.Vector3Field(fieldRect, GUIContent.none, prevLocal);
            if (editedLocal != prevLocal)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Transform t = (Transform)targets[i];
                    Undo.RecordObject(t, "Change Local Position");
                    t.localPosition = editedLocal;
                    EditorUtility.SetDirty(t);
                }
            }

            Rect resetRect = new Rect(fieldRect.xMax + spacing, rect.y + (rect.height - resetSize) / 2f, resetSize, resetSize);
            if (GUI.Button(resetRect, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Transform t = (Transform)targets[i];
                    Undo.RecordObject(t, "Reset Local Position");
                    t.localPosition = Vector3.zero;
                    EditorUtility.SetDirty(t);
                }
            }

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

                Vector3 prevWorld = first.position;
                Vector3 editedWorld = EditorGUI.Vector3Field(fieldRect2, GUIContent.none, prevWorld);
                if (editedWorld != prevWorld)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform t = (Transform)targets[i];
                        Undo.RecordObject(t, "Change World Position");
                        t.position = editedWorld;
                        EditorUtility.SetDirty(t);
                    }
                }

                // Reset aligné
                Rect resetRect2 = new Rect(fieldRect2.xMax + spacing, rect2.y + (rect2.height - resetSize) / 2f, resetSize, resetSize);
                if (GUI.Button(resetRect2, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform t = (Transform)targets[i];
                        Undo.RecordObject(t, "Reset World Position");
                        t.position = Vector3.zero;
                        EditorUtility.SetDirty(t);
                    }
                }

                EditorGUILayout.Space(1);
            }
        }

        void DrawRotation(Transform first)
        {
            EditorGUIUtility.labelWidth = labelWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            Rect foldRect = new Rect(rect.x + 16, rect.y, 14f, rect.height);
            isRotationExpanded = EditorGUI.Foldout(foldRect, isRotationExpanded, GUIContent.none, true);

            Rect labelRect = new Rect(rect.x + 14f + spacing, rect.y, labelWidth - 14f - spacing, rect.height);
            EditorGUI.LabelField(labelRect, "Rotation");

            float fieldWidth = rect.width - (labelWidth + resetSize + spacing * 4 + spaceBetweenLabelAndField);
            Rect fieldRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField, rect.y, fieldWidth, rect.height);

            Vector3 prevLocalEuler = first.localEulerAngles;
            Vector3 editedLocalEuler = EditorGUI.Vector3Field(fieldRect, GUIContent.none, prevLocalEuler);
            if (editedLocalEuler != prevLocalEuler)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Transform t = (Transform)targets[i];
                    Undo.RecordObject(t, "Change Local Rotation");
                    t.localRotation = Quaternion.Euler(editedLocalEuler);
                    EditorUtility.SetDirty(t);
                }
            }

            Rect resetRect = new Rect(fieldRect.xMax + spacing, rect.y + (rect.height - resetSize) / 2f, resetSize, resetSize);
            if (GUI.Button(resetRect, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Transform t = (Transform)targets[i];
                    Undo.RecordObject(t, "Reset Local Rotation");
                    t.localRotation = Quaternion.identity;
                    EditorUtility.SetDirty(t);
                }
            }

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

                Vector3 prevWorldEuler = first.eulerAngles;
                Vector3 editedWorldEuler = EditorGUI.Vector3Field(fieldRect2, GUIContent.none, prevWorldEuler);
                if (editedWorldEuler != prevWorldEuler)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform t = (Transform)targets[i];
                        Undo.RecordObject(t, "Change World Rotation");
                        t.rotation = Quaternion.Euler(editedWorldEuler);
                        EditorUtility.SetDirty(t);
                    }
                }

                // Reset aligné
                Rect resetRect2 = new Rect(fieldRect2.xMax + spacing, rect2.y + (rect2.height - resetSize) / 2f, resetSize, resetSize);
                if (GUI.Button(resetRect2, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform t = (Transform)targets[i];
                        Undo.RecordObject(t, "Reset World Rotation");
                        t.eulerAngles = Vector3.zero;
                        EditorUtility.SetDirty(t);
                    }
                }

                EditorGUILayout.Space(1);
            }
        }
        
        void DrawScale(Transform first)
        {
            EditorGUIUtility.labelWidth = labelWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            Rect foldRect = new Rect(rect.x + 16, rect.y, 14f, rect.height);
            isScaleExpanded = EditorGUI.Foldout(foldRect, isScaleExpanded, GUIContent.none, true);

            Rect labelRect = new Rect(rect.x + 14f + spacing, rect.y, labelWidth - 14f - spacing, rect.height);
            EditorGUI.LabelField(labelRect, "Scale");


            Rect linkButtonRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField - spaceBetweenLinkIconAndField, rect.y + (rect.height - 14f) / 2f - 2, 16f, 16f);
            if (GUI.Button(linkButtonRect, EditorGUIUtility.IconContent(isScaleLinked ? "d_Linked" : "d_Unlinked"), IconButtonStyle))
                isScaleLinked = !isScaleLinked;

            float fieldWidth = rect.width - (labelWidth + resetSize + spacing * 4 + spaceBetweenLabelAndField);
            Rect fieldRect = new Rect(rect.x + labelWidth + spaceBetweenLabelAndField, rect.y, fieldWidth, rect.height);
            
            Vector3 prevLocal = first.localScale;
            Vector3 editedLocal = EditorGUI.Vector3Field(fieldRect, GUIContent.none, prevLocal);
            if (editedLocal != prevLocal)
            {
                if (isScaleLinked)
                {
                    const float eps = 1e-6f;
                    // Determine which component changed based on the first selected transform
                    bool xChanged = Mathf.Abs(editedLocal.x - prevLocal.x) > eps;
                    bool yChanged = Mathf.Abs(editedLocal.y - prevLocal.y) > eps;
                    // zChanged as fallback

                    if (xChanged)
                    {
                        float factor = Mathf.Abs(prevLocal.x) > eps ? editedLocal.x / prevLocal.x : 0f;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            Transform t = (Transform)targets[i];
                            Vector3 tPrev = t.localScale;
                            Undo.RecordObject(t, "Change Local Scale");
                            if (Mathf.Abs(tPrev.x) > eps)
                                t.localScale = tPrev * factor;
                            else
                                t.localScale = Vector3.one * editedLocal.x;
                            EditorUtility.SetDirty(t);
                        }
                    }
                    else if (yChanged)
                    {
                        float factor = Mathf.Abs(prevLocal.y) > eps ? editedLocal.y / prevLocal.y : 0f;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            Transform t = (Transform)targets[i];
                            Vector3 tPrev = t.localScale;
                            Undo.RecordObject(t, "Change Local Scale");
                            if (Mathf.Abs(tPrev.y) > eps)
                                t.localScale = tPrev * factor;
                            else
                                t.localScale = Vector3.one * editedLocal.y;
                            EditorUtility.SetDirty(t);
                        }
                    }
                    else // z changed (ou fallback)
                    {
                        float factor = Mathf.Abs(prevLocal.z) > eps ? editedLocal.z / prevLocal.z : 0f;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            Transform t = (Transform)targets[i];
                            Vector3 tPrev = t.localScale;
                            Undo.RecordObject(t, "Change Local Scale");
                            if (Mathf.Abs(tPrev.z) > eps)
                                t.localScale = tPrev * factor;
                            else
                                t.localScale = Vector3.one * editedLocal.z;
                            EditorUtility.SetDirty(t);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform t = (Transform)targets[i];
                        Undo.RecordObject(t, "Change Local Scale");
                        t.localScale = editedLocal;
                        EditorUtility.SetDirty(t);
                    }
                }
            }
            
            Rect resetRect = new Rect(fieldRect.xMax + spacing, rect.y + (rect.height - resetSize) / 2f, resetSize, resetSize);
            if (GUI.Button(resetRect, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Transform t = (Transform)targets[i];
                    Undo.RecordObject(t, "Reset Local Scale");
                    t.localScale = Vector3.one;
                    EditorUtility.SetDirty(t);
                }
            }

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
                
                Vector3 worldScale = first.lossyScale;
                Vector3 newWorldScale = EditorGUI.Vector3Field(fieldRect2, GUIContent.none, worldScale);
                if (newWorldScale != worldScale)
                {
                    if (isScaleLinked)
                    {
                        const float eps = 1e-6f;
                        bool xChanged = Mathf.Abs(newWorldScale.x - worldScale.x) > eps;
                        bool yChanged = Mathf.Abs(newWorldScale.y - worldScale.y) > eps;

                        if (xChanged)
                        {
                            float ratio = Mathf.Abs(worldScale.x) > eps ? newWorldScale.x / worldScale.x : 0f;
                            for (int i = 0; i < targets.Length; i++)
                            {
                                Transform t = (Transform)targets[i];
                                Undo.RecordObject(t, "Change World Scale");
                                Vector3 tWorld = t.lossyScale;
                                if (Mathf.Abs(tWorld.x) > eps)
                                    SetWorldScale(t, tWorld * ratio);
                                else
                                    SetWorldScale(t, Vector3.one * newWorldScale.x);
                                EditorUtility.SetDirty(t);
                            }
                        }
                        else if (yChanged)
                        {
                            float ratio = Mathf.Abs(worldScale.y) > eps ? newWorldScale.y / worldScale.y : 0f;
                            for (int i = 0; i < targets.Length; i++)
                            {
                                Transform t = (Transform)targets[i];
                                Undo.RecordObject(t, "Change World Scale");
                                Vector3 tWorld = t.lossyScale;
                                if (Mathf.Abs(tWorld.y) > eps)
                                    SetWorldScale(t, tWorld * ratio);
                                else
                                    SetWorldScale(t, Vector3.one * newWorldScale.y);
                                EditorUtility.SetDirty(t);
                            }
                        }
                        else
                        {
                            float ratio = Mathf.Abs(worldScale.z) > eps ? newWorldScale.z / worldScale.z : 0f;
                            for (int i = 0; i < targets.Length; i++)
                            {
                                Transform t = (Transform)targets[i];
                                Undo.RecordObject(t, "Change World Scale");
                                Vector3 tWorld = t.lossyScale;
                                if (Mathf.Abs(tWorld.z) > eps)
                                    SetWorldScale(t, tWorld * ratio);
                                else
                                    SetWorldScale(t, Vector3.one * newWorldScale.z);
                                EditorUtility.SetDirty(t);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < targets.Length; i++)
                        {
                            Transform t = (Transform)targets[i];
                            Undo.RecordObject(t, "Change World Scale");
                            SetWorldScale(t, newWorldScale);
                            EditorUtility.SetDirty(t);
                        }
                    }
                }

                // Reset aligné
                Rect resetRect2 = new Rect(fieldRect2.xMax + spacing, rect2.y + (rect2.height - resetSize) / 2f, resetSize, resetSize);
                if (GUI.Button(resetRect2, EditorGUIUtility.IconContent("Refresh"), IconButtonStyle))
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform t = (Transform)targets[i];
                        Undo.RecordObject(t, "Reset World Scale");
                        SetWorldScale(t, Vector3.one);
                        EditorUtility.SetDirty(t);
                    }
                }

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