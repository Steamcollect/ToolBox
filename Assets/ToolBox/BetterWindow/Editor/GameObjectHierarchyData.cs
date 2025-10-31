using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterWindow
{
    [System.Serializable]
    public class GameObjectHierarchyData
    {
        public Object obj;
        GameObject go;

        public Component[] components;

        int instanceID;

        public GameObjectHierarchyData(Object obj)
        {
            if (obj == null)
                return;

            this.obj = obj;

            go = obj as GameObject;
            instanceID = go.GetInstanceID();
            components = go.GetComponents<Component>();
        }

        public void Draw(Rect selectionRect)
        {
            // Check if components have changed
            Component[] comps = go.GetComponents<Component>();
            if (components.Length != comps.Length ||
                (comps.Length > 1 && comps[1] != components[1]))
            {
                components = comps;
            }

            // Draw background
            bool isSelected = Selection.instanceIDs.Contains(instanceID);
            Color bgColor = isSelected
                ? new Color(0.172549f, .3647059f, .5294118f)
                : (EditorGUIUtility.isProSkin
                    ? new Color(0.219f, 0.219f, 0.219f)
                    : new Color(0.76f, 0.76f, 0.76f));

            // icon
            Rect iconRect = 
                ToolBoxPreferences.s_DrawModeInHierarchy == ToolBoxIconDrawModeInHierarchy.OnGameObjectIcon ?
                    new Rect(selectionRect.x - 1, selectionRect.y, 16, 16) :
                    new Rect(selectionRect.x + selectionRect.width - 17, selectionRect.y, 16, 16);
            EditorGUI.DrawRect(iconRect, bgColor);

            GUI.DrawTexture(
                iconRect,
                components.Length <= 1 ?
                    EditorGUIUtility.IconContent("Folder Icon").image :
                    EditorGUIUtility.ObjectContent(null, components[1].GetType()).image as Texture2D,
                ScaleMode.ScaleToFit);
        }
    }

    public enum ToolBoxIconDrawModeInHierarchy
    {
        OnGameObjectIcon,
        OnRightSide,
    }
}