using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterInterface
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
            if(ToolBoxPreferences.s_DrawModeInHierarchy == ToolBoxIconDrawModeInHierarchy.UnityDefault)
                return;

            // Check if components have changed
            Component[] comps = go.GetComponents<Component>();
            if (components.Length != comps.Length ||
                (comps.Length > 1 && comps[1] != components[1]))
            {
                components = comps;
            }

            if(components.Length <= 1 && !ToolBoxPreferences.s_DrawFolderIconInHierarchy)
                return;

            if (components.Length > 1 && components[1] == null) 
                return;

            // Draw background
            bool isSelected = Selection.instanceIDs.Contains(instanceID);
            Color bgColor = isSelected
                ? new Color(0.172549f, .3647059f, .5294118f)
                : (EditorGUIUtility.isProSkin
                    ? new Color(0.219f, 0.219f, 0.219f)
                    : new Color(0.76f, 0.76f, 0.76f));

            // icon
            Rect iconRect = new Rect(selectionRect.x - 1, selectionRect.y, 16, 16);

            if(ToolBoxPreferences.s_DrawModeInHierarchy == ToolBoxIconDrawModeInHierarchy.ToolBoxDefault)
            {
                if (components.Length <= 1)
                    iconRect = new Rect(selectionRect.x - 1, selectionRect.y, 16, 16);
                else
                    iconRect = new Rect(selectionRect.x + selectionRect.width - 17, selectionRect.y, 16, 16);
            }

            if(ToolBoxPreferences.s_DrawModeInHierarchy != ToolBoxIconDrawModeInHierarchy.ToolBoxDefault || components.Length <= 1)
                EditorGUI.DrawRect(iconRect, bgColor);

            Texture icon = null;
            if (components.Length > 1)
                icon = EditorGUIUtility.ObjectContent(null, components[1].GetType()).image as Texture2D;
            else
                icon = EditorGUIUtility.IconContent("Folder Icon").image;

            if (icon == null) return;
            GUI.DrawTexture(
                iconRect,
                icon,
                ScaleMode.ScaleToFit);
        }
    }

    public enum ToolBoxIconDrawModeInHierarchy
    {
        ToolBoxDefault,
        OverrideGameObjectIcon,
        UnityDefault,
    }
}