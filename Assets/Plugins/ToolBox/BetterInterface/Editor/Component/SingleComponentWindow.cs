using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterInterface
{
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

        private float windowHeight;
        Vector2 minMaxWindowHeight = new Vector2(30, 800);

        public Rect newPosition;

        public static void Show(Component component)
        {
            if (component == null)
            {
                Debug.LogWarning("Aucun composant fourni.");
                return;
            }

            var window = CreateInstance<SingleComponentWindow>();
            window.targetComponent = component;
            window.titleContent = new GUIContent(component.gameObject.name);
            window.CreateEditor();

            // Taille initiale fixe
            float initialWidth = 400;
            float initialHeight = 200;

            Rect rect = new Rect(
                (Screen.currentResolution.width - initialWidth) / 2f,
                (Screen.currentResolution.height - initialHeight) / 2f,
                initialWidth,
                initialHeight
            );

            window.newPosition = rect;

            window.minSize = new Vector2(300, 150);
            window.maxSize = new Vector2(500, 800);

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

            position = newPosition;
        }

        private void DrawInspectorScrollable()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (componentEditor == null)
                CreateEditor();

            componentEditor.OnInspectorGUI();

            GUILayout.Space(3);

            if (Event.current.type == EventType.Repaint)
            {

                Rect lastRect = GUILayoutUtility.GetLastRect();
                windowHeight = lastRect.yMax + 25;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeaderWithCloseAndDrag()
        {
            Rect headerRect = new Rect(0, 0, position.width, HeaderHeight);
            EditorGUI.DrawRect(headerRect, new Color(0.18f, 0.18f, 0.18f));

            // Tilte
            GUIContent title = new GUIContent(
                "    " + targetComponent.gameObject.name + " : " + targetComponent.GetType().Name,
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
                        newPosition = new Rect(windowStartPos.x + delta.x, windowStartPos.y + delta.y, position.width, position.height);
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

            if (!isResizing) 
                newPosition.height = Mathf.Clamp(windowHeight, minMaxWindowHeight.x, minMaxWindowHeight.y);
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
                        newPosition = new Rect(position.x, position.y, newWidth, position.height);
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

        private void OnDisable()
        {
            if (componentEditor != null)
                DestroyImmediate(componentEditor);
        }
    }
}