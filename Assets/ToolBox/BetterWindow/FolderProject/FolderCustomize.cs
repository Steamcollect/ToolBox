using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ToolBox.BetterWindow
{
    [InitializeOnLoad]
    public static class FolderCustomizer
    {
        //private static Dictionary<string, FolderData> folderSettings = new Dictionary<string, FolderData>();
        //private const string SaveKey = "FolderCustomizer_Data";
    
        //static FolderCustomizer() // Constructeur
        //{
        //    LoadFoldersData();
        //    InitializeAllFolders();
        //    EditorApplication.projectWindowItemOnGUI += DrawFolders;
        //}

        //private static void InitializeAllFolders()
        //{
        //    string[] allFolders = AssetDatabase.GetSubFolders("Assets");

        //    Queue<string> queue = new Queue<string>(allFolders);
        //    while (queue.Count > 0)
        //    {
        //        string folder = queue.Dequeue();
        //        if (!folderSettings.ContainsKey(folder))
        //            folderSettings[folder] = new FolderData().SetPath(folder);

        //        string[] subFolders = AssetDatabase.GetSubFolders(folder);
        //        foreach (string sub in subFolders)
        //            queue.Enqueue(sub);
        //    }

        //    SaveFoldersData();
        //}

        //private static void DrawFolders(string guid, Rect rect) // Draw folder base on path
        //{
        //    string path = AssetDatabase.GUIDToAssetPath(guid);
        //    if (!AssetDatabase.IsValidFolder(path))
        //        return;

        //    if (folderSettings.TryGetValue(path, out FolderData data))
        //    {
        //        // Couleur de fond
        //        EditorGUI.DrawRect(rect, data.backgroundColor);

        //        Color prevColor = GUI.color;
                
        //        // Icône personnalisée
        //        if (data.icon != null)
        //        {
        //            Rect iconRect = new Rect(rect.x, rect.y, 16, 16);
        //            rect.x += 17;
        //            rect.y -= 1;

        //            prevColor = GUI.color;
        //            GUI.color = data.iconColor;
        //            GUI.DrawTexture(iconRect, data.icon, ScaleMode.ScaleToFit);
        //            GUI.color = prevColor;
        //        }

        //        // Couleur du texte
        //        prevColor = GUI.color;
        //        GUI.color = data.textColor;

        //        // Nom du dossier
        //        EditorGUI.LabelField(rect, Path.GetFileName(path));

        //        GUI.color = prevColor;
        //    }
        //}

        //public static FolderData GetOrCreateData(string path)
        //{
        //    if (!folderSettings.TryGetValue(path, out FolderData data))
        //    {
        //        data = new FolderData().SetPath(path);
        //        folderSettings[path] = data;
        //    }
        //    return data;
        //}

        //public static void SetData(string path, Color background, Color text, Texture2D icon, Color iconColor)
        //{
        //    FolderData data = GetOrCreateData(path);
        //    data.backgroundColor = background;
        //    data.textColor = text;
        //    data.icon = icon;
        //    data.iconColor = iconColor;
        //    folderSettings[path] = data;

        //    EditorApplication.RepaintProjectWindow();
        //}

        //public static void ResetAllFoldersData()
        //{
        //    var keys = new List<string>(folderSettings.Keys);
        //    foreach (string key in keys)
        //        folderSettings[key] = new FolderData().SetPath(key);

        //    EditorApplication.RepaintProjectWindow();
        //}

        //public static void SaveFoldersData()
        //{
        //    FolderDataList list = new FolderDataList(folderSettings.Values);
        //    string json = JsonUtility.ToJson(list, true);
        //    EditorPrefs.SetString(SaveKey, json);
        //}

        //private static void LoadFoldersData()
        //{
        //    folderSettings.Clear();
        //    string json = EditorPrefs.GetString(SaveKey, "");
        //    if (!string.IsNullOrEmpty(json))
        //    {
        //        var list = JsonUtility.FromJson<FolderDataList>(json);
        //        if (list?.items != null)
        //        {
        //            foreach (var item in list.items)
        //                folderSettings[item.path] = item;
        //        }
        //    }
        //}
        //private class FolderDataList
        //{
        //    public List<FolderData> items = new List<FolderData>();
        //    public FolderDataList(IEnumerable<FolderData> data)
        //    {
        //        items.AddRange(data);
        //    }
        //}
    }
}