using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MVsToolkitProjectEditor
{
    static Dictionary<string /*path*/, MVsToolkitProjectItem /*item*/> items = new();

    static MVsToolkitProjectEditor()
    {
        EditorApplication.projectWindowItemOnGUI += OnGUI;
    }

    static void OnGUI(string guid, Rect selectionRect)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path))
            return;
    }
}