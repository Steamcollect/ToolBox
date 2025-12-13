using UnityEngine;

[CreateAssetMenu(fileName = "SSO_ItemData", menuName = "MVsToolkitDemo/SSO_ItemData")]
public class SSO_ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;

    public Sprite visual;
    public GameObject prefab;
}