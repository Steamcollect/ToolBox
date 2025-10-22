using ToolBox.BetterWindow;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DefaultAsset)), CanEditMultipleObjects]
public class FolderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Récupère tous les chemins sélectionnés
        string[] paths = new string[targets.Length];
        for (int i = 0; i < targets.Length; i++)
            paths[i] = AssetDatabase.GetAssetPath(targets[i]);

        // Vérifie si au moins un est un dossier
        if (!System.Array.Exists(paths, AssetDatabase.IsValidFolder))
        {
            DrawDefaultInspector();
            return;
        }

        EditorGUILayout.LabelField("Folder Customization", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // On utilise le premier dossier comme référence
        FolderData firstData = FolderCustomizer.GetOrCreateData(paths[0]);

        Color newBgColor = firstData.backgroundColor;
        Color newTextColor = firstData.textColor;
        Texture2D newIcon = firstData.icon;
        Color newIconColor = firstData.iconColor;
        
        bool modified = false;

        EditorGUI.BeginChangeCheck();
        GUI.enabled = true;
        
        // === BACKGROUND ===
        EditorGUILayout.BeginHorizontal();
        newBgColor = EditorGUILayout.ColorField("Background", newBgColor);
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            newBgColor = firstData.GetInitBkColor();
            modified = true;
        }
        EditorGUILayout.EndHorizontal();

        // === TEXT COLOR ===
        EditorGUILayout.BeginHorizontal();
        newTextColor = EditorGUILayout.ColorField("Text Color", newTextColor);
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            newTextColor = firstData.GetInitTextColor();
            modified = true;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);

        // === ICON ===
        EditorGUILayout.BeginHorizontal();
        newIcon = (Texture2D)EditorGUILayout.ObjectField("Folder Icon", newIcon, typeof(Texture2D), false);
        if (newIcon == null) newIcon = firstData.GetInitIcon();
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            newIcon = firstData.GetInitIcon();
            modified = true;
        }
        EditorGUILayout.EndHorizontal();

        // === ICON COLOR ===
        EditorGUILayout.BeginHorizontal();
        newIconColor = EditorGUILayout.ColorField("Icon Color", newIconColor);
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            newIconColor = firstData.GetInitIconColor();
            modified = true;
        }
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck() || modified)
        {
            foreach (string path in paths)
            {
                if (AssetDatabase.IsValidFolder(path))
                    FolderCustomizer.SetData(path, newBgColor, newTextColor, newIcon, newIconColor);
            }
            FolderCustomizer.SaveFoldersData();
            EditorApplication.RepaintProjectWindow();
        }

        EditorGUILayout.Space(50);

        if (GUILayout.Button("Reset All Folders"))
        {
            if (EditorUtility.DisplayDialog("Reset All Folders", "Reset ALL folders?", "Yes", "Cancel"))
            {
                FolderCustomizer.ResetAllFoldersData();
                FolderCustomizer.SaveFoldersData();
                EditorApplication.RepaintProjectWindow();
            }
        }
        
        GUI.enabled = true;
    }
}