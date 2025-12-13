using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BetterInterface
{
    [InitializeOnLoad]
    public static class MVsHierarchyInputController
    {
        static MVsHierarchyInputController()
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
            }
        }
    }
}
