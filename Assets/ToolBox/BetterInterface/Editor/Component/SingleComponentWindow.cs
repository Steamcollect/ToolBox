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

        private int dragControlId;

        private float windowHeight;
        Vector2 minMaxWindowHeight = new Vector2(30, 800);

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
            float initialWidth = 300;
            float initialHeight = 200;

            Rect rect = new Rect(
                (Screen.currentResolution.width - initialWidth) / 2f,
                (Screen.currentResolution.height - initialHeight) / 2f,
                initialWidth,
                initialHeight
            );

            window.position = rect;

            // Bornes de redimensionnement
            window.minSize = new Vector2(150, 100);
            window.maxSize = new Vector2(500, 800);

            window.ShowUtility();
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

            position = new Rect(position.x, position.y,
                450, Mathf.Clamp(windowHeight, minMaxWindowHeight.x, minMaxWindowHeight.y));
        }

        private void DrawInspectorScrollable()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (componentEditor == null)
                CreateEditor();

            componentEditor.OnInspectorGUI();

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

            GUIContent title = new GUIContent(
                "    " + targetComponent.GetType().Name,
                AssetPreview.GetMiniThumbnail(targetComponent)
            );

            GUI.Label(new Rect(5, 2, position.width - 40, 20), title, EditorStyles.boldLabel);
            HandleDrag(headerRect);

            GUILayout.Space(HeaderHeight);
        }

        void HandleDrag(Rect headerRect)
        {
            dragControlId = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;
            EventType typeForCtrl = e.GetTypeForControl(dragControlId);

            switch (typeForCtrl)
            {
                case EventType.MouseDown:
                    if (headerRect.Contains(e.mousePosition) && e.button == 0)
                    {
                        GUIUtility.hotControl = dragControlId;
                        isDragging = true;

                        // Point de départ écran pour un drag fiable
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
        }

        private void OnDisable()
        {
            if (componentEditor != null)
                DestroyImmediate(componentEditor);
        }
    }
}