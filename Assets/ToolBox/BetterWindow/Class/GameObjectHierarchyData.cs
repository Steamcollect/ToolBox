using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GameObjectHierarchyData
{
    public string iconName;

    [System.NonSerialized] public int InstanceID;
    [System.NonSerialized] public GameObject GameObject;

    [System.NonSerialized] public Texture2D icon;

    //public Texture2D Icon;

    public GameObjectHierarchyData(int instanceId, GameObject gameObject)
    {
        InstanceID = instanceId;
        GameObject = gameObject;

        icon = (Texture2D)EditorGUIUtility.IconContent("GameObject Icon").image;
    }

    public void SetIcon(string iconName)
    {
        this.iconName = iconName;
        icon = (Texture2D)EditorGUIUtility.IconContent(iconName).image;
    }

    public void Draw(Rect selectionRect)
    {
        bool isSelected = Selection.instanceIDs.Contains(InstanceID);
        Color bgColor = isSelected
            ? new Color(0.172549f, .3647059f, .5294118f)
            : (EditorGUIUtility.isProSkin
                ? new Color(0.219f, 0.219f, 0.219f)
                : new Color(0.76f, 0.76f, 0.76f));

        Rect iconRect = new Rect(selectionRect.x - 1, selectionRect.y, 16, 16);
        EditorGUI.DrawRect(iconRect, bgColor);
        GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
    }
}