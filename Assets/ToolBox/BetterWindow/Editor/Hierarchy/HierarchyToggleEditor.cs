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
        // R�cup�rer le GameObject associ�
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        Rect toggleRect = new Rect(selectionRect.x - 20, selectionRect.y - 1, 18, 18);

        if (obj != null && 
            (selectionRect.Contains(Event.current.mousePosition) || toggleRect.Contains(Event.current.mousePosition)))
        {
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