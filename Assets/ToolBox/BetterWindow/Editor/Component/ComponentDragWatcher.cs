using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ComponentDragWatcher
{
    static bool isDragging = false;

    static ComponentDragWatcher()
    {
        EditorApplication.update += CheckDrag;
    }

    static void CheckDrag()
    {
        if (!isDragging && DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is Component comp)
        {
            isDragging = true;
            SingleComponentWindow win = SingleComponentWindow.Show(comp);
            win.SetPosition(GetMouseScreenPosition());
            win.StartDrag(GetMouseScreenPosition());
        }
        else if (isDragging && DragAndDrop.objectReferences.Length == 0)
        {
            isDragging = false;
        }
    }

    static Vector2 GetMouseScreenPosition()
    {
        if (EditorWindow.mouseOverWindow != null)
        {
            Vector2 local = Event.current != null ? Event.current.mousePosition : GUIUtility.GUIToScreenPoint(Vector2.zero);
            return EditorWindow.mouseOverWindow.position.position + local;
        }

        return new Vector2(Screen.currentResolution.width / 2f, Screen.currentResolution.height / 2f);
    }
}
