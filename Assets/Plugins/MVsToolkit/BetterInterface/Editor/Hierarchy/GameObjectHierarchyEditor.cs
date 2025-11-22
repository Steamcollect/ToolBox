using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

namespace MVsToolkit.BetterInterface
{
    [InitializeOnLoad]
    public static class GameObjectHierarchyEditor
    {
        static int iconSize = 16;
        static int iconsSpacing = 0;

        static GameObjectHierarchyEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        static void OnHierarchyGUI(int instanceID, Rect rect)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj == null) return;

            Draw(
                obj as GameObject, 
                Selection.instanceIDs.Contains(instanceID), 
                rect);
        }

        static void Draw(GameObject go, bool isSelected, Rect rect)
        {
            if (go == null) return;

            Component[] comps = go.GetComponents<Component>();
            Rect iconRect;
            Texture icon;
            Event e = Event.current;

            bool isHover = rect.Contains(e.mousePosition);

            Color bgColor = UnityWindowHelper.HierarchyBackgroundColor(((int)rect.y / (int)rect.height) % 2 == 1);
            if (isHover) bgColor = UnityWindowHelper.HierarchyHoverColor;
            if (isSelected) bgColor = UnityWindowHelper.HierarchySelectionColor;

            EditorGUI.DrawRect(new Rect(rect.x - 28, rect.y, rect.width + 44, rect.height), bgColor);
            EditorGUI.LabelField(new Rect(rect.x + iconSize, rect.y, rect.width + 44, rect.height), go.name);


            if (comps.Length <= 1)
            {
                if(MVsToolkitPreferences.s_DrawFolderIconInHierarchy)
                {
                    iconRect = new Rect(rect.x - 1, rect.y, iconSize, iconSize);
                    icon = EditorGUIUtility.IconContent("Folder Icon").image;

                    EditorGUI.DrawRect(iconRect, bgColor);
                    GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                }
            }
            else
            {
                if (MVsToolkitPreferences.s_OverrideGameObjectIcon)
                {
                    iconRect = new Rect(
                            rect.x - 1,
                            rect.y,
                            iconSize,
                            iconSize);

                    EditorGUI.DrawRect(iconRect, bgColor);
                    DrawComponentIcon(iconRect, comps[1], e, false);
                }

                if (MVsToolkitPreferences.s_ShowComponentsIcon)
                {
                    for (int i = 1; i < comps.Length; i++)
                    {
                        iconRect = new Rect(
                            rect.x + rect.width - ((comps.Length - i) * iconSize) + ((comps.Length - i) * -iconsSpacing),
                            rect.y,
                            iconSize,
                            iconSize);

                        DrawComponentIcon(iconRect, comps[i], e, true);
                    }
                }                    
            }
        }

        static void DrawComponentIcon(Rect rect, Component comp, Event e, bool isInteractible)
        {
            Texture icon = EditorGUIUtility.ObjectContent(null, comp.GetType()).image as Texture2D;
            if (icon == null) return;

            GUIStyle iconStyle = new GUIStyle();
            iconStyle.padding = new RectOffset(0, 0, 0, 0);
            iconStyle.margin = new RectOffset(0, 0, 0, 0);
            iconStyle.border = new RectOffset(0, 0, 0, 0);

            GUIContent content = new GUIContent(icon, comp.GetType().Name);
            GUI.Label(rect, content, iconStyle);

            if (isInteractible)
            {
                if (e.alt && e.type == EventType.MouseDown && e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        SingleComponentWindow.Show(comp);
                        e.Use();
                    }
                }
            }
        }
    }
}