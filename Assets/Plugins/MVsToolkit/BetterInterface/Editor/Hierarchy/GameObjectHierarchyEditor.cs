using System.Linq;
using UnityEditor;
using UnityEngine;
using static PlasticGui.WorkspaceWindow.Items.ExpandedTreeNode;

namespace MVsToolkit.BetterInterface
{
    [InitializeOnLoad]
    public static class GameObjectHierarchyEditor
    {
        static int iconSize = 16;
        static int iconsSpacing = 2;

        static GameObjectHierarchyEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj == null) return;

            Draw(
                obj as GameObject, 
                Selection.instanceIDs.Contains(instanceID), 
                selectionRect);
        }

        static void Draw(GameObject go, bool isSelected, Rect selectionRect)
        {
            if (go == null) return;

            Component[] comps = go.GetComponents<Component>();
            Rect iconRect;
            Texture icon;

            if (comps.Length <= 1)
            {
                if(ToolBoxPreferences.s_DrawFolderIconInHierarchy)
                {
                    Color bgColor = isSelected
                        ? new Color(0.172549f, .3647059f, .5294118f)
                        : (EditorGUIUtility.isProSkin
                            ? new Color(0.219f, 0.219f, 0.219f)
                            : new Color(0.76f, 0.76f, 0.76f));

                    iconRect = new Rect(selectionRect.x - 1, selectionRect.y, iconSize, iconSize);
                    icon = EditorGUIUtility.IconContent("Folder Icon").image;

                    GUI.DrawTexture(
                        iconRect,
                        icon,
                        ScaleMode.ScaleToFit);
                }
            }
            else 
            {
                for (int i = 1; i < comps.Length; i++)
                {
                    iconRect = new Rect(
                        selectionRect.x + selectionRect.width - ((comps.Length - i) * iconSize) + ((comps.Length - i) * iconsSpacing), 
                        selectionRect.y, 
                        iconSize, 
                        iconSize);

                    icon = EditorGUIUtility.ObjectContent(null, comps[i].GetType()).image as Texture2D;

                    if(icon == null) continue;

                    GUIStyle iconStyle = new GUIStyle();
                    iconStyle.padding = new RectOffset(0, 0, 0, 0);
                    iconStyle.margin = new RectOffset(0, 0, 0, 0);
                    iconStyle.border = new RectOffset(0, 0, 0, 0);

                    GUIContent content = new GUIContent(icon, comps[i].GetType().Name);
                    GUI.Label(iconRect, content, iconStyle);

                    Event e = Event.current;
                    if (e.alt && e.type == EventType.MouseDown && e.button == 0)
                    {
                        if (iconRect.Contains(e.mousePosition))
                        {
                            SingleComponentWindow.Show(comps[i]);
                            e.Use();
                        }
                    }
                }
            }
        }
    }
}