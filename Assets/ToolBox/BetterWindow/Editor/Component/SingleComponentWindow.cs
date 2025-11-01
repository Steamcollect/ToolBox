using UnityEditor;
using UnityEngine;

public class SingleComponentWindow : EditorWindow
{
    private Component targetComponent;
    private Editor componentEditor;
    private Vector2 scrollPos;

    private bool isDragging;
    private Vector2 dragStartScreen;
    private Vector2 windowStartPos;
    private const float HeaderHeight = 22f;

    private bool isResizing;
    private float resizeStartScreenX;
    private float windowStartWidth;
    private const float ResizeHandleSize = 6f;

    private int dragControlId;
    private int resizeControlId;

    public static void Show(Component component)
    {
        if (component == null)
        {
            Debug.LogWarning("Aucun composant fourni.");
            return;
        }

        var window = CreateInstance<SingleComponentWindow>();
        window.targetComponent = component;
        window.titleContent = new GUIContent(
            component.GetType().Name,
            AssetPreview.GetMiniThumbnail(component)
        );
        window.CreateEditor();

        float initialWidth = 400f;
        float inspectorH = window.CalculateInspectorHeight(initialWidth - 8f);
        float height = 22f + inspectorH + 6f;

        Rect rect = new Rect(
            (Screen.currentResolution.width - initialWidth) / 2f,
            (Screen.currentResolution.height - height) / 2f,
            initialWidth,
            height
        );

        window.position = rect;
        window.minSize = new Vector2(250, 100);
        window.ShowPopup();
    }

    private void CreateEditor()
    {
        if (targetComponent != null)
            componentEditor = Editor.CreateEditor(targetComponent);
    }

    private void OnGUI()
    {
        if (targetComponent == null)
        {
            EditorGUILayout.HelpBox("Aucun composant à afficher.", MessageType.Info);
            return;
        }

        if (componentEditor == null)
            CreateEditor();

        DrawHeaderWithCloseAndDrag();
        DrawInspectorScrollable();

        HandleResizeHorizontal();
    }

    private void DrawInspectorScrollable()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUI.BeginChangeCheck();
        componentEditor.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(targetComponent);
            AdjustHeightToContent();
            Repaint();
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawHeaderWithCloseAndDrag()
    {
        Rect headerRect = new Rect(0, 0, position.width, HeaderHeight);
        EditorGUI.DrawRect(headerRect, new Color(0.18f, 0.18f, 0.18f));

        // Tilte
        GUIContent title = new GUIContent(
            "    " +  targetComponent.gameObject.name + " : " + targetComponent.GetType().Name,
            AssetPreview.GetMiniThumbnail(targetComponent)
        );
        GUI.Label(new Rect(5, 2, position.width - 40, 20), title, EditorStyles.boldLabel);

        // Close
        Rect closeRect = new Rect(position.width - 22, 2, 18, 18);
        var closeStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16
        };

        EditorGUIUtility.AddCursorRect(closeRect, MouseCursor.Link);
        if (GUI.Button(closeRect, "×", closeStyle))
        {
            Close();
            GUIUtility.ExitGUI();
        }

        HandleDrag(closeRect, headerRect);

        GUILayout.Space(HeaderHeight);
    }

    void HandleDrag(Rect closeRect, Rect headerRect)
    {
        dragControlId = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;
        EventType typeForCtrl = e.GetTypeForControl(dragControlId);

        if (!closeRect.Contains(e.mousePosition))
            EditorGUIUtility.AddCursorRect(headerRect, MouseCursor.MoveArrow);

        switch (typeForCtrl)
        {
            case EventType.MouseDown:
                if (headerRect.Contains(e.mousePosition) && !closeRect.Contains(e.mousePosition) && e.button == 0)
                {
                    GUIUtility.hotControl = dragControlId;
                    isDragging = true;

                    // Point de départ côté écran pour ne pas perdre le drag en sortant de la fenêtre
                    dragStartScreen = GUIUtility.GUIToScreenPoint(e.mousePosition);
                    windowStartPos = position.position;
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == dragControlId && isDragging)
                {
                    Vector2 curScreen = GUIUtility.GUIToScreenPoint(e.mousePosition);
                    Vector2 delta = curScreen - dragStartScreen;
                    position = new Rect(windowStartPos.x + delta.x, windowStartPos.y + delta.y, position.width, position.height);
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == dragControlId)
                {
                    GUIUtility.hotControl = 0;
                    isDragging = false;
                    e.Use();
                }
                break;
        }

        if(!isResizing) AdjustHeightToContent();
    }

    private void HandleResizeHorizontal()
    {
        Rect handleRect = new Rect(position.width - ResizeHandleSize, 0, ResizeHandleSize, position.height);
        EditorGUIUtility.AddCursorRect(handleRect, MouseCursor.ResizeHorizontal);

        resizeControlId = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;
        EventType typeForCtrl = e.GetTypeForControl(resizeControlId);

        switch (typeForCtrl)
        {
            case EventType.MouseDown:
                if (handleRect.Contains(e.mousePosition) && e.button == 0)
                {
                    GUIUtility.hotControl = resizeControlId;
                    isResizing = true;

                    resizeStartScreenX = GUIUtility.GUIToScreenPoint(e.mousePosition).x;
                    windowStartWidth = position.width;
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == resizeControlId && isResizing)
                {
                    float curScreenX = GUIUtility.GUIToScreenPoint(e.mousePosition).x;
                    float deltaX = curScreenX - resizeStartScreenX;
                    float newWidth = Mathf.Max(250f, windowStartWidth + deltaX);
                    position = new Rect(position.x, position.y, newWidth, position.height);
                    Repaint();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == resizeControlId)
                {
                    GUIUtility.hotControl = 0;
                    isResizing = false;
                    e.Use();
                }
                break;
        }
    }

    private float CalculateInspectorHeight(float availableWidth)
    {
        if (targetComponent == null) return 100f;

        var so = new SerializedObject(targetComponent);
        var prop = so.GetIterator();

        float total = 0f;
        bool enterChildren = true;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        while (prop.NextVisible(enterChildren))
        {
            // Calcule la hauteur réelle de la propriété (respecte les menus déroulants)
            float ph = EditorGUI.GetPropertyHeight(prop, includeChildren: prop.isExpanded);
            total += ph + spacing;
            enterChildren = false;
        }

        // Marge interne
        total += 4f;
        return total;
    }
    private void AdjustHeightToContent()
    {
        float contentWidth = Mathf.Max(200f, position.width - 8f);
        float inspectorH = CalculateInspectorHeight(contentWidth);
        float desired = 22f + inspectorH + 6f; // header + contenu + marge bas

        // Évite qu'elle dépasse trop l’écran
        desired = Mathf.Min(desired, Screen.currentResolution.height * 0.9f);

        if (Mathf.Abs(position.height - desired) > 0.5f)
            position = new Rect(position.x, position.y, position.width, desired);
    }

    private void OnInspectorUpdate()
    {
        if (targetComponent != null)
            AdjustHeightToContent();
    }

    private void OnDisable()
    {
        if (componentEditor != null)
            DestroyImmediate(componentEditor);
    }
}