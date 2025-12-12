using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform), true), CanEditMultipleObjects]
public class MVsCustomTransform : Editor
{
    bool isPositionExpanded = false;
    bool isRotationExpanded = false;
    bool isScaleExpanded = false;
    bool lockUniformScale = true;

    // Shared style for icon-only buttons to ensure icons are centered and have no background
    private static GUIStyle s_iconButtonStyle;
    private GUIStyle IconButtonStyle
    {
        get
        {
            if (s_iconButtonStyle == null)
            {
                s_iconButtonStyle = new GUIStyle();
                s_iconButtonStyle.padding = new RectOffset(0, 0, 0, 0);
                s_iconButtonStyle.margin = new RectOffset(0, 0, 0, 0);
                s_iconButtonStyle.border = new RectOffset(0, 0, 0, 0);
                s_iconButtonStyle.alignment = TextAnchor.MiddleCenter;
                s_iconButtonStyle.imagePosition = ImagePosition.ImageOnly;
                // Ensure no background is drawn
                s_iconButtonStyle.normal.background = null;
                s_iconButtonStyle.hover.background = null;
                s_iconButtonStyle.active.background = null;
                s_iconButtonStyle.focused.background = null;
            }
            return s_iconButtonStyle;
        }
    }

    // Fixed width for foldout label area so expanded rows can align their fields
    private const float kFoldoutLabelWidth = 110f;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(2);

        // Use the first target for display values
        Transform firstTransform = (Transform)target;

        GUIContent resetIcon = EditorGUIUtility.IconContent("Refresh");

        // ===== POSITION =====
        EditorGUILayout.BeginHorizontal();
        // Use Foldout overload without GUILayout options
        isPositionExpanded = EditorGUILayout.Foldout(isPositionExpanded, isPositionExpanded ? "Local Position" : "Position", true);

        // Determine mixed value state for local position across selected objects
        bool mixedLocal = false;
        Vector3 localValue = firstTransform.localPosition;
        if (targets.Length > 1)
        {
            for (int i = 1; i < targets.Length; i++)
            {
                if (((Transform)targets[i]).localPosition != localValue)
                {
                    mixedLocal = true;
                    break;
                }
            }
        }

        EditorGUI.showMixedValue = mixedLocal;
        EditorGUI.BeginChangeCheck();
        Vector3 newLocalQuick = EditorGUILayout.Vector3Field(GUIContent.none, localValue, GUILayout.ExpandWidth(true));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Edit Local Position");
            foreach (Object obj in targets)
            {
                ((Transform)obj).localPosition = newLocalQuick;
            }
        }
        EditorGUI.showMixedValue = false;

        if (GUILayout.Button(resetIcon, IconButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
        {
            Undo.RecordObjects(targets, "Reset Local Position");
            foreach (Object obj in targets)
            {
                ((Transform)obj).localPosition = Vector3.zero;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (isPositionExpanded)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("World Position", GUILayout.Width(kFoldoutLabelWidth));

            bool mixedWorld = false;
            Vector3 worldValue = firstTransform.position;
            if (targets.Length > 1)
            {
                for (int i = 1; i < targets.Length; i++)
                {
                    if (((Transform)targets[i]).position != worldValue)
                    {
                        mixedWorld = true;
                        break;
                    }
                }
            }

            EditorGUI.showMixedValue = mixedWorld;
            EditorGUI.BeginChangeCheck();
            Vector3 newWorld = EditorGUILayout.Vector3Field(GUIContent.none, worldValue, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Edit World Position");
                foreach (Object obj in targets)
                {
                    ((Transform)obj).position = newWorld;
                }
            }
            EditorGUI.showMixedValue = false;

            if (GUILayout.Button(resetIcon, IconButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
            {
                Undo.RecordObjects(targets, "Reset World Position");
                foreach (Object obj in targets)
                {
                    ((Transform)obj).position = Vector3.zero;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(2); // same spacing as position

        // ===== ROTATION =====
        EditorGUILayout.BeginHorizontal();
        isRotationExpanded = EditorGUILayout.Foldout(isRotationExpanded, isRotationExpanded ? "Local Rotation" : "Rotation", true);

        // Local rotation (Euler) quick field
        bool mixedLocalRot = false;
        Vector3 localEuler = firstTransform.localEulerAngles;
        if (targets.Length > 1)
        {
            for (int i = 1; i < targets.Length; i++)
            {
                if (((Transform)targets[i]).localEulerAngles != localEuler)
                {
                    mixedLocalRot = true;
                    break;
                }
            }
        }

        EditorGUI.showMixedValue = mixedLocalRot;
        EditorGUI.BeginChangeCheck();
        Vector3 newLocalRotQuick = EditorGUILayout.Vector3Field(GUIContent.none, localEuler, GUILayout.ExpandWidth(true));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Edit Local Rotation");
            foreach (Object obj in targets)
            {
                ((Transform)obj).localEulerAngles = newLocalRotQuick;
            }
        }
        EditorGUI.showMixedValue = false;

        if (GUILayout.Button(resetIcon, IconButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
        {
            Undo.RecordObjects(targets, "Reset Local Rotation");
            foreach (Object obj in targets)
            {
                ((Transform)obj).localEulerAngles = Vector3.zero;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (isRotationExpanded)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("World Rotation", GUILayout.Width(kFoldoutLabelWidth));

            bool mixedWorldRot = false;
            Vector3 worldEuler = firstTransform.eulerAngles;
            if (targets.Length > 1)
            {
                for (int i = 1; i < targets.Length; i++)
                {
                    if (((Transform)targets[i]).eulerAngles != worldEuler)
                    {
                        mixedWorldRot = true;
                        break;
                    }
                }
            }

            EditorGUI.showMixedValue = mixedWorldRot;
            EditorGUI.BeginChangeCheck();
            Vector3 newWorldEuler = EditorGUILayout.Vector3Field(GUIContent.none, worldEuler, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Edit World Rotation");
                foreach (Object obj in targets)
                {
                    ((Transform)obj).rotation = Quaternion.Euler(newWorldEuler);
                }
            }
            EditorGUI.showMixedValue = false;

            if (GUILayout.Button(resetIcon, IconButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
            {
                Undo.RecordObjects(targets, "Reset World Rotation");
                foreach (Object obj in targets)
                {
                    ((Transform)obj).rotation = Quaternion.identity;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(2); // same spacing as position

        // ===== SCALE =====
        EditorGUILayout.BeginHorizontal();
        isScaleExpanded = EditorGUILayout.Foldout(isScaleExpanded, isScaleExpanded ? "Local Scale" : "Scale", true);

        // Local scale quick field
        bool mixedLocalScale = false;
        Vector3 localScale = firstTransform.localScale;
        if (targets.Length > 1)
        {
            for (int i = 1; i < targets.Length; i++)
            {
                if (((Transform)targets[i]).localScale != localScale)
                {
                    mixedLocalScale = true;
                    break;
                }
            }
        }

        // Toggle icon for uniform scale
        GUIContent lockIcon = lockUniformScale ? EditorGUIUtility.IconContent("LockIcon-On") : EditorGUIUtility.IconContent("LockIcon");
        if (GUILayout.Button(lockIcon, IconButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
        {
            lockUniformScale = !lockUniformScale;
        }

        EditorGUI.showMixedValue = mixedLocalScale;
        EditorGUI.BeginChangeCheck();
        Vector3 newLocalScale = EditorGUILayout.Vector3Field(GUIContent.none, localScale, GUILayout.ExpandWidth(true));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Edit Local Scale");
            // Handle uniform scaling if locked
            for (int t = 0; t < targets.Length; t++)
            {
                Transform tr = (Transform)targets[t];
                Vector3 old = tr.localScale;
                Vector3 applied = newLocalScale;
                if (lockUniformScale)
                {
                    // Determine which component changed relative to firstTransform's value
                    // If original component is zero, fall back to setting all components to that new value
                    if (newLocalScale.x != localScale.x && localScale.x != 0f)
                    {
                        float ratio = newLocalScale.x / localScale.x;
                        applied = old * ratio;
                    }
                    else if (newLocalScale.y != localScale.y && localScale.y != 0f)
                    {
                        float ratio = newLocalScale.y / localScale.y;
                        applied = old * ratio;
                    }
                    else if (newLocalScale.z != localScale.z && localScale.z != 0f)
                    {
                        float ratio = newLocalScale.z / localScale.z;
                        applied = old * ratio;
                    }
                    else if (localScale.x == 0f || localScale.y == 0f || localScale.z == 0f)
                    {
                        // If any original is zero, just set uniformly to the x component (or y/z if changed)
                        float uniform = newLocalScale.x != localScale.x ? newLocalScale.x : (newLocalScale.y != localScale.y ? newLocalScale.y : newLocalScale.z);
                        applied = new Vector3(uniform, uniform, uniform);
                    }
                }

                tr.localScale = applied;
            }
        }
        EditorGUI.showMixedValue = false;

        if (GUILayout.Button(resetIcon, IconButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
        {
            Undo.RecordObjects(targets, "Reset Local Scale");
            foreach (Object obj in targets)
            {
                ((Transform)obj).localScale = Vector3.one;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (isScaleExpanded)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("World Scale", GUILayout.Width(kFoldoutLabelWidth));

            // world lossyScale (read-only for parents; editing will set localScale to match desired world scale)
            bool mixedWorldScale = false;
            Vector3 worldScale = firstTransform.lossyScale;
            if (targets.Length > 1)
            {
                for (int i = 1; i < targets.Length; i++)
                {
                    if (((Transform)targets[i]).lossyScale != worldScale)
                    {
                        mixedWorldScale = true;
                        break;
                    }
                }
            }

            EditorGUI.showMixedValue = mixedWorldScale;
            EditorGUI.BeginChangeCheck();
            Vector3 newWorldScale = EditorGUILayout.Vector3Field(GUIContent.none, worldScale, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Edit World Scale");
                for (int t = 0; t < targets.Length; t++)
                {
                    Transform tr = (Transform)targets[t];
                    Vector3 parentScale = tr.parent ? tr.parent.lossyScale : Vector3.one;
                    Vector3 appliedLocal = tr.localScale;

                    if (lockUniformScale)
                    {
                        // Determine ratio based on changed component relative to first's world scale
                        if (newWorldScale.x != worldScale.x && worldScale.x != 0f)
                        {
                            float ratio = newWorldScale.x / worldScale.x;
                            appliedLocal = tr.localScale * ratio;
                        }
                        else if (newWorldScale.y != worldScale.y && worldScale.y != 0f)
                        {
                            float ratio = newWorldScale.y / worldScale.y;
                            appliedLocal = tr.localScale * ratio;
                        }
                        else if (newWorldScale.z != worldScale.z && worldScale.z != 0f)
                        {
                            float ratio = newWorldScale.z / worldScale.z;
                            appliedLocal = tr.localScale * ratio;
                        }
                        else if (worldScale.x == 0f || worldScale.y == 0f || worldScale.z == 0f)
                        {
                            float uniform = newWorldScale.x != worldScale.x ? newWorldScale.x : (newWorldScale.y != worldScale.y ? newWorldScale.y : newWorldScale.z);
                            appliedLocal = new Vector3(uniform, uniform, uniform);
                        }
                    }
                    else
                    {
                        // Compute local = desiredWorld / parentWorld (per component), avoid divide by zero
                        appliedLocal = new Vector3(
                            parentScale.x != 0f ? newWorldScale.x / parentScale.x : newWorldScale.x,
                            parentScale.y != 0f ? newWorldScale.y / parentScale.y : newWorldScale.y,
                            parentScale.z != 0f ? newWorldScale.z / parentScale.z : newWorldScale.z
                        );
                    }

                    tr.localScale = appliedLocal;
                }
            }
            EditorGUI.showMixedValue = false;

            if (GUILayout.Button(resetIcon, IconButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
            {
                Undo.RecordObjects(targets, "Reset World Scale");
                foreach (Object obj in targets)
                {
                    Transform tr = (Transform)obj;
                    Vector3 parentScale = tr.parent ? tr.parent.lossyScale : Vector3.one;
                    Vector3 newLocal = new Vector3(
                        parentScale.x != 0f ? 1f / parentScale.x : 1f,
                        parentScale.y != 0f ? 1f / parentScale.y : 1f,
                        parentScale.z != 0f ? 1f / parentScale.z : 1f
                    );
                    tr.localScale = newLocal;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
     
        EditorGUILayout.Space(2);
    }
}