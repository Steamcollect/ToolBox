using UnityEditor;
using UnityEngine;

namespace MVsToolkit.SceneBrowser
{
    public class SceneBrowserPopUp : PopupWindowContent
    {
        int searchHeight = 20;
        int maxHeight = 300;
        string searchTxt;

        public override void OnOpen()
        {
            SceneBrowserContent.RefreshScenesList();
        }

        public override void OnGUI(Rect rect)
        {
            searchTxt = EditorGUILayout.TextField(string.Empty, searchTxt);

            Rect contentR = new Rect(rect.x, rect.y + searchHeight + 12, rect.width, Mathf.Min(maxHeight, rect.height - searchHeight));
            SceneBrowserContent.OnGUI(contentR, searchTxt);
        }

        public override Vector2 GetWindowSize()
        {
            return SceneBrowserContent.GetWindowSize();
        }
    }
}