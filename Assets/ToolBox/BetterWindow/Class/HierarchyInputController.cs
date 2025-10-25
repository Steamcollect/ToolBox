using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.SceneManagement;

namespace ToolBox.BetterWindow
{
    [InitializeOnLoad]
    public static class HierarchyInputController
    {
        static HierarchyInputController()
        {
            // S’enregistre sur chaque redraw de la Hierarchy
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (Application.isPlaying) return;

            Event e = Event.current;
            Object obj = EditorUtility.InstanceIDToObject(instanceID);

            if (e.type == EventType.MouseDown && e.button == 0 && selectionRect.Contains(e.mousePosition)) // On left click
            {
                if (obj == null)
                {
                    Rect popUpRect = new Rect();
                    popUpRect.x = selectionRect.x;
                    popUpRect.y = selectionRect.y + 18;

                    PopupWindow.Show(popUpRect, new SceneBrowserPopUp());
                }
                else
                {
                    if (e.alt)
                    {
                        Rect popUpRect = new Rect();

                        popUpRect.x = e.mousePosition.x + 50;
                        popUpRect.y = e.mousePosition.y - 50;

                        PopupWindow.Show(
                            popUpRect, 
                            new CustomGameObjectHierarchyPopUp(
                                obj, 
                                GameObjectHierarchyEditor.GetCurrentSceneGlobalId().ToString()
                            )
                        );
                    }
                }
            }
        }
    }
}
