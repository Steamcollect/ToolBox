using UnityEditor;
using UnityEngine;

public class SingleComponentInspectorWindow : EditorWindow
{
    private Component targetComponent;
    private Editor componentEditor;
    private bool isDragging;
    private Vector2 dragOffset;
    private const float HeaderHeight = 24f;
    private const float WindowWidth = 450f;

    public static SingleComponentInspectorWindow ShowPopupAt(Vector2 screenPos)
    {
        var window = CreateInstance<SingleComponentInspectorWindow>();
        window.titleContent = new GUIContent("Locked Inspector");
        window.ShowPopup();

        // Position après affichage
        var rect = new Rect(screenPos.x, screenPos.y, WindowWidth, 200f);
        window.position = rect;
        return window;
    }

    private void OnGUI()
    {
        DrawHeader();

        GUILayout.Space(HeaderHeight + 4);

        if (targetComponent == null)
        {
            EditorGUILayout.HelpBox("Drag & Drop a Component to inspect it.", MessageType.Info);
            return;
        }

        if (componentEditor == null)
            componentEditor = Editor.CreateEditor(targetComponent);

        EditorGUILayout.ObjectField("Component", targetComponent, typeof(Component), true);
        EditorGUILayout.Space();
        componentEditor.OnInspectorGUI();
    }

    private void DrawHeader()
    {
        Rect headerRect = new Rect(0, 0, position.width, HeaderHeight);
        GUI.Box(headerRect, GUIContent.none, EditorStyles.toolbar);

        GUI.Label(new Rect(8, 2, position.width - 50, HeaderHeight - 4), "Locked Inspector", EditorStyles.boldLabel);

        // Bouton fermer
        if (GUI.Button(new Rect(position.width - 22, 2, 20, HeaderHeight - 4), "✕", EditorStyles.toolbarButton))
            Close();

        // Drag
        HandleHeaderDrag(headerRect);
    }

    private void HandleHeaderDrag(Rect headerRect)
    {
        Event e = Event.current;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (headerRect.Contains(e.mousePosition) && e.button == 0)
                {
                    isDragging = true;
                    dragOffset = e.mousePosition;
                    e.Use();
                }
                break;

            //case EventType.MouseDrag:
            //    if (isDragging && e.button == 0)
            //    {
            //        Vector2 mouseScreen = GUIUtility.GUIToScreenPoint(e.mousePosition);
            //        var newRect = position;
            //        newRect.position = mouseScreen - dragOffset;
            //        position = newRect;
            //        e.Use();
            //    }
            //    break;

            case EventType.MouseUp:
                if (isDragging && e.button == 0)
                {
                    isDragging = false;
                    e.Use();
                }
                break;
        }

        if (isDragging)
        {
            Vector2 mouseScreen = GUIUtility.GUIToScreenPoint(e.mousePosition);
            var newRect = position;
            newRect.position = mouseScreen - dragOffset;
            position = newRect;
            e.Use();
        }
    }

    private void SetTarget(Component comp)
    {
        targetComponent = comp;
        if (componentEditor != null)
            DestroyImmediate(componentEditor);
        componentEditor = Editor.CreateEditor(targetComponent);
        Repaint();
    }

    public void SetTargetPublic(Component comp) => SetTarget(comp);
}