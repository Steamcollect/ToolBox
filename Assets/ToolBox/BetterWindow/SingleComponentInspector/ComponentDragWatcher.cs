using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ComponentDragWatcher
{
    static bool isDragging = false;
    static Component currentComponent;

    static ComponentDragWatcher()
    {
        //EditorApplication.update += CheckDrag;
    }

    static void CheckDrag()
    {
        // Début du drag
        if (!isDragging && DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is Component comp)
        {
            isDragging = true;
            currentComponent = comp;
        }
        // Fin du drag
        else if (isDragging && DragAndDrop.objectReferences.Length == 0)
        {
            isDragging = false;

            Vector2 screenPos = GetMouseScreenPosition();

            var window = SingleComponentInspectorWindow.ShowPopupAt(screenPos);
            if (currentComponent != null)
                window.SetTargetPublic(currentComponent);

            currentComponent = null;
        }
    }

    // ✅ Compatible toutes plateformes Unity
    static Vector2 GetMouseScreenPosition()
    {
        // 1. Si une fenêtre est sous la souris (Scene, Game, Inspector, etc.)
        if (EditorWindow.mouseOverWindow != null)
        {
            // Convertit la position locale en position écran
            Vector2 local = Event.current != null ? Event.current.mousePosition : GUIUtility.GUIToScreenPoint(Vector2.zero);
            return EditorWindow.mouseOverWindow.position.position + local;
        }

        // 2. Fallback : position approximative (milieu de l’écran)
        return new Vector2(Screen.currentResolution.width / 2f, Screen.currentResolution.height / 2f);
    }
}
