using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyToggleEditor
{
    static HierarchyToggleEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        // Récupérer le GameObject associé
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        Rect detectionCheckBox = new Rect(selectionRect.x - 20, selectionRect.y, 30, 18);

        if (obj != null && detectionCheckBox.Contains(Event.current.mousePosition))
        {
            Rect toggleRect = new Rect(selectionRect.x - 20, selectionRect.y, 18, 18);

            bool newActive = GUI.Toggle(toggleRect, obj.activeSelf, GUIContent.none);

            if (newActive != obj.activeSelf)
            {
                Undo.RecordObject(obj, "Toggle Active State");
                obj.SetActive(newActive);
                EditorUtility.SetDirty(obj);
            }
        }
    }
}
