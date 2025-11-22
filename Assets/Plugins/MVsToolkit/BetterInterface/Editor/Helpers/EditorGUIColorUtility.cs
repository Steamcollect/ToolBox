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

    public static Color PrefabColor(bool isActive)
    {
        if (EditorGUIUtility.isProSkin)
            return isActive ? new Color(0.4132075f, 0.8216152f, 1) : new Color(0.466949f, 0.6383449f, 0.7132075f);
        else
            return isActive ? new Color(0.4f, 0.6f, 0.4f) : new Color(0.8f, 0.4f, 0.4f);
    }
}