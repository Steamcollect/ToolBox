using UnityEngine;
using UnityEditor;

namespace ToolBox.BetterWindow
{
    [System.Serializable]
    public class FolderData
    {
        public string path;
        public Color backgroundColor;
        public Color textColor;
        public Texture2D icon;
        public Color iconColor;

        public FolderData()
        {
            backgroundColor = GetInitBkColor();
            textColor = GetInitTextColor();
            icon = GetInitIcon();
            iconColor = GetInitIconColor();
        }

        public FolderData SetPath(string path)
        {
            this.path = path;
            return this;
        }

        public Color GetInitBkColor()
        {
            return new Color(.2196079f, .2196079f, .2196079f, 1f);
        }
        public Color GetInitTextColor()
        {
            return Color.white;
        }
        public Texture2D GetInitIcon()
        {
            return (Texture2D)EditorGUIUtility.IconContent("Folder Icon").image;
        }

        public Color GetInitIconColor()
        {
            return Color.white;
        }
    }
}