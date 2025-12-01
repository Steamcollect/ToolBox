using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BetterInterface
{
    [InitializeOnLoad]
    public static class MVsToolkitHierarchyEditor
    {
        static int iconSize = 16;
        static int iconsSpacing = 0;

        static MVsToolkitHierarchyEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        static void OnHierarchyGUI(int instanceID, Rect rect)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj == null) return;

            Draw(instanceID,
                obj as GameObject, 
                Selection.instanceIDs.Contains(instanceID), 
                rect);
        }

        static void Draw(int instanceID, GameObject go, bool isSelected, Rect rect)
        {
            if (go == null) return;

            Component[] comps = go.GetComponents<Component>();

            Rect iconRect;
            Texture icon;
            Event e = Event.current;

            bool isHover = rect.Contains(e.mousePosition);

            bool isPrefab = go.IsPartOfAnyPrefab();
            bool isMissingPrefab = go.IsPartOfMissingPrefab();

            Color bgColor = MVsToolkitColorUtility.HierarchyBackgroundColor(((int)rect.y / (int)rect.height) % 2 == 1);
            if (isHover) bgColor = MVsToolkitColorUtility.HierarchyHoverColor;
            if (isSelected) bgColor = MVsToolkitColorUtility.HierarchySelectionColor;

            EditorGUI.DrawRect(new Rect(rect.x - 21, rect.y, rect.width + 22, rect.height), bgColor);
            if (MVsToolkitPreferences.s_IsChildLine) 
                EditorGUI.DrawRect(new Rect(rect.x - 22, rect.y, 1, rect.height), new Color(.3f,.3f,.3f));

            DrawSetActiveToggle(go, rect, e);

            SetGUIColor(go, isSelected, true);
            EditorGUI.LabelField(new Rect(rect.x + iconSize, rect.y, rect.width + 44, rect.height), go.name);

            if (go.transform.childCount > 0)
            {
                GUI.color = Color.white;
                Rect foldoutRect = new Rect(rect.x - 14f, rect.y, 14f, rect.height);
                EditorGUI.Foldout(foldoutRect, IsGameObjectExpand(instanceID), GUIContent.none, false);
            }

            if (comps.Length <= 1)
            {
                if(MVsToolkitPreferences.s_DrawFolderIconInHierarchy)
                {
                    iconRect = new Rect(rect.x - 1, rect.y, iconSize, iconSize);
                    GUI.color = Color.white;

                    if(isPrefab && isMissingPrefab)
                        DrawErrorIcon(iconRect);
                    else
                    {
                        icon = EditorGUIUtility.IconContent("Folder Icon").image;

                        EditorGUI.DrawRect(iconRect, bgColor);

                        SetGUIColor(go, isSelected, true);
                        GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                    }
                }
                else
                {
                    iconRect = new Rect(
                        rect.x - 1,
                        rect.y,
                        iconSize,
                        iconSize);

                    SetGUIColor(go, isSelected);
                    DrawGameObjectIcon(go, isPrefab, isMissingPrefab, iconRect);
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

                    GUI.color = Color.white;
                    EditorGUI.DrawRect(iconRect, bgColor);

                    SetGUIColor(go, isSelected);

                    if(isPrefab && isMissingPrefab)
                        DrawErrorIcon(iconRect);
                    else
                        DrawComponentIcon(iconRect, comps[1], e, false);
                }
                else
                {
                    iconRect = new Rect(
                        rect.x - 1,
                        rect.y,
                        iconSize,
                        iconSize);

                    SetGUIColor(go, isSelected);
                    DrawGameObjectIcon(go, isPrefab, isMissingPrefab, iconRect);
                }

                if (MVsToolkitPreferences.s_ShowComponentsIcon)
                {
                    SetGUIColor(go, isSelected);

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

            GUI.color = Color.white;
        }

        static void DrawGameObjectIcon(GameObject go, bool isPrefab, bool isMissingPrefab, Rect rect)
        {
            Texture icon;

            if (isPrefab)
            {
                if(isMissingPrefab)
                    icon = EditorGUIUtility.IconContent("console.erroricon").image;
                else
                    icon = EditorGUIUtility.IconContent("Prefab Icon").image;
            }
            else
            {
                icon = EditorGUIUtility.IconContent("GameObject Icon").image;
            }

            GUI.DrawTexture(rect, icon);
        }

        static void DrawErrorIcon(Rect rect)
        {
            Texture icon = EditorGUIUtility.IconContent("console.erroricon").image;
            if (icon == null) return;
            GUIStyle iconStyle = new GUIStyle();
            iconStyle.padding = new RectOffset(0, 0, 0, 0);
            iconStyle.margin = new RectOffset(0, 0, 0, 0);
            iconStyle.border = new RectOffset(0, 0, 0, 0);

            GUI.DrawTexture(rect, icon);
        }
        static void DrawComponentIcon(Rect rect, Component comp, Event e, bool isInteractible)
        {
            GUIStyle iconStyle = new GUIStyle();
            iconStyle.padding = new RectOffset(0, 0, 0, 0);
            iconStyle.margin = new RectOffset(0, 0, 0, 0);
            iconStyle.border = new RectOffset(0, 0, 0, 0);

            Texture icon;
            GUIContent content;

            if (comp == null)
            {
                icon = EditorGUIUtility.IconContent("console.erroricon").image;
                content = new GUIContent(icon, "Missing Component");
                GUI.Label(rect, content, iconStyle);

                return;
            }

            icon = EditorGUIUtility.ObjectContent(null, comp.GetType()).image as Texture2D;
            if (icon == null) return;

            content = new GUIContent(icon, comp.GetType().Name + (isInteractible ? "   <color=grey>Alt+LClick</color>" : ""));
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

        static void DrawSetActiveToggle(GameObject go, Rect rect, Event e)
        {
            Rect toggleRect = new Rect(rect.x - 27, rect.y - 1, 18, 18);

            if (rect.Contains(e.mousePosition) || toggleRect.Contains(e.mousePosition))
            {
                bool newActive = GUI.Toggle(toggleRect, go.activeSelf, GUIContent.none);

                if (newActive != go.activeSelf)
                {
                    Undo.RecordObject(go, "Toggle Active State");
                    go.SetActive(newActive);
                    EditorUtility.SetDirty(go);
                }
            }
        }

        static void SetGUIColor(GameObject go, bool isSelected, bool usePrefabColor = false)
        {
            if (usePrefabColor && go.IsPartOfAnyPrefab())
            {
                GUI.color = MVsToolkitColorUtility.PrefabColor(GetTopParentActiveSelf(go), isSelected, go.IsPartOfMissingPrefab());
            }
            else
            {
                GUI.color = GetTopParentActiveSelf(go) ? Color.white : Color.grey;
            }
        }

        static bool IsGameObjectExpand(int instanceID)
        {
            System.Type sceneHierarchyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            System.Reflection.MethodInfo getExpandedIDs = sceneHierarchyWindowType.GetMethod("GetExpandedIDs", BindingFlags.NonPublic | BindingFlags.Instance);

            // Déclaration explicite du type
            PropertyInfo lastInteractedHierarchyWindow =
                sceneHierarchyWindowType.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Public | BindingFlags.Static);

            if (lastInteractedHierarchyWindow == null)
            {
                return false;
            }

            int[] expandedIDs = getExpandedIDs.Invoke(lastInteractedHierarchyWindow.GetValue(null), null) as int[];

            return expandedIDs != null && expandedIDs.Contains(instanceID);
        }
        static bool IsPartOfAnyPrefab(this GameObject obj)
        {
            return PrefabUtility.IsPartOfAnyPrefab(obj);
        }
        static bool IsPartOfMissingPrefab(this GameObject go)
        {
            var root = PrefabUtility.GetNearestPrefabInstanceRoot(go);
            if (root == null) return false;

            var status = PrefabUtility.GetPrefabInstanceStatus(root);
            return status != PrefabInstanceStatus.Connected;
        }
        static bool GetTopParentActiveSelf(GameObject go)
        {
            if (go == null) return false;

            Transform t = go.transform;

            while (t.parent != null)
            {
                if (!t.gameObject.activeSelf)
                    return false;

                t = t.parent;
            }

            return t.gameObject.activeSelf;
        }
    }
}