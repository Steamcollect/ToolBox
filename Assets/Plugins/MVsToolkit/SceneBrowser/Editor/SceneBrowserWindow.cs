using MVsToolkit.SceneBrowser;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.SceneBrowser
{
    public class SceneBrowserWindow : EditorWindow
    {
        int searchHeight = 20;
        string searchTxt;

        [MenuItem("Window/MVsToolkit/Scene Browser")]
        public static void ShowWindow()
        {
            GetWindow<SceneBrowserWindow>("Scene Browser");
        }

        private void OnGUI()
        {
            // --- Barre de recherche + bouton W ---
            GUILayout.BeginHorizontal();
            searchTxt = EditorGUILayout.TextField(string.Empty, searchTxt);

            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh")))
            {
                SceneBrowserContent.RefreshScenesList();
            }
            GUILayout.EndHorizontal();

            // --- Zone de contenu ---
            float contentY = searchHeight + 12;
            float contentHeight = position.height - contentY;

            Rect contentR = new Rect(
                0,
                contentY,
                position.width,
                contentHeight
            );

            // Même API que le popup, mais sans maxContentHeight
            SceneBrowserContent.DrawContent(contentR, searchTxt, contentHeight);
        }
    }
}