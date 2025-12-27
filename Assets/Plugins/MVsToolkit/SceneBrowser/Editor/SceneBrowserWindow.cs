using UnityEditor;
using UnityEngine;

namespace MVsToolkit.SceneBrowser
{
    public class SceneBrowserWindow : EditorWindow
    {
        int searchHeight = 20;
        string searchTxt;

        GUIStyle iconStyle;

        [MenuItem("Window/MVsToolkit/Scene Browser")]
        public static void ShowWindow()
        {
            GetWindow<SceneBrowserWindow>("Scene Browser");
        }

        private void OnGUI()
        {
            if(iconStyle == null)
            {
                iconStyle = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(2, 2, 2, 2),
                    margin = new RectOffset(0, 0, 0, 0),
                    alignment = TextAnchor.MiddleCenter
                };
            }

            GUILayout.BeginHorizontal();
            searchTxt = EditorGUILayout.TextField(string.Empty, searchTxt);

            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(searchHeight * 1.5f)))
            {
                SceneBrowserContent.RefreshScenesList();
            }
            GUILayout.EndHorizontal();

            float contentY = searchHeight + 12;
            float contentHeight = position.height - contentY;

            Rect contentR = new Rect(
                0,
                contentY,
                position.width,
                contentHeight
            );

            SceneBrowserContent.DrawContent(contentR, searchTxt, contentHeight);
        }
    }
}