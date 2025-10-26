using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneHierarchyMeta : ScriptableObject
{
    public List<GameObjectHierarchyData> items = new();

    public string sceneGuid;
    public string sceneGlobalId;
    public SceneAsset sceneRef;
}