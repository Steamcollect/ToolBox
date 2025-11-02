using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ComponentDragAndDropWatcher
{
    static bool isDragging = false;
    static Component component;

    static ComponentDragAndDropWatcher()
    {
        EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
        if(!isDragging 
            && EditorInputManager.LeftMouse && EditorInputManager.Alt 
            && DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is Component)
        {
            isDragging = true;
            component = DragAndDrop.objectReferences[0] as Component;
        }
        else if(isDragging 
            && DragAndDrop.objectReferences.Length <= 0)
        {
            isDragging = false;

            SingleComponentWindow.Show(component);
            component = null;
        }
    }
}