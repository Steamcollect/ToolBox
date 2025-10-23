using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GameObjectHierarchyData
{
    public int InstanceID;
    public GameObject GameObject;
    public Texture2D icon;

    //public Texture2D Icon;

    public GameObjectHierarchyData(int instanceId, GameObject gameObject)
    {
        InstanceID = instanceId;
        GameObject = gameObject;

        icon = (Texture2D)EditorGUIUtility.IconContent("GameObject Icon").image;
    }
}