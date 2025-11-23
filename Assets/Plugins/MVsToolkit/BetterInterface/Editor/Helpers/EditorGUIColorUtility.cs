using MVsToolkit.BetterInterface;
using UnityEditor;
using UnityEngine;

public static class EditorGUIColorUtility
{
    public static Color HierarchyBackgroundColor(bool isOdd = false)
    {
        if (EditorGUIUtility.isProSkin)
            return (MVsToolkitPreferences.s_IsZebraMode && isOdd) ? 
                new Color(0.2f, 0.2f, 0.2f) : new Color(0.219f, 0.219f, 0.219f);
        else
            return (MVsToolkitPreferences.s_IsZebraMode && isOdd) ? 
                new Color(0.92f, 0.92f, 0.92f) : new Color(0.76f, 0.76f, 0.76f);
    }

    public static Color HierarchyHoverColor
    {
        get
        {
            if (EditorGUIUtility.isProSkin)
                return new Color(0.2666667f, 0.2666667f, 0.2666667f);
            else
                return new Color(0.8f, 0.8f, 0.8f);
        }
    }

    public static Color HierarchySelectionColor
    {
        get
        {
            if (EditorGUIUtility.isProSkin)
                return new Color(0.172549f, 0.3647059f, 0.5294118f);
            else
                return new Color(0.24f, 0.49f, 0.90f);
        }
    }

    public static Color PrefabColor(bool isActive, bool isSelected)
    {
        if (isActive)
        {
            if (isSelected)
            {
                return EditorGUIUtility.isProSkin ?
                    Color.white :
                    Color.black;
            }
            else
            {
                return EditorGUIUtility.isProSkin ?
                    new Color(0.55f, 0.78f, 1.0f) :
                    new Color(0.48f, 0.70f, 1.0f);
            }
        }
        else
        {
            if (isSelected)
            {
                return EditorGUIUtility.isProSkin ?
                    new Color(0.55f, 0.78f, 1.0f) :
                    new Color(0.48f, 0.70f, 1.0f);
            }
            else
            {
                return EditorGUIUtility.isProSkin ?
                    new Color(0.63f, 0.67f, 0.74f) :
                    new Color(0.74f, 0.78f, 0.85f);
            }
        }
    }
}