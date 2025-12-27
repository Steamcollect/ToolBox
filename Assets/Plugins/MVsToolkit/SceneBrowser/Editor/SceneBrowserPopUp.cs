using UnityEditor;
using UnityEngine;

namespace MVsToolkit.SceneBrowser
{
    public class SceneBrowserPopUp : PopupWindowContent
    {
        int searchHeight = 20;
        int maxContentHeight = 300;
        string searchTxt;

        public override void OnOpen()
        {
            SceneBrowserContent.RefreshScenesList();
        }

        public override void OnGUI(Rect rect)
        {
            searchTxt = EditorGUILayout.TextField(string.Empty, searchTxt);

            float contentY = searchHeight + 12;
            float contentHeight = Mathf.Min(maxContentHeight, rect.height - contentY);

            Rect contentR = new Rect(
                rect.x,
                rect.y + contentY,
                rect.width,
                contentHeight
            );

            SceneBrowserContent.DrawContent(contentR, searchTxt);
        }


        public override Vector2 GetWindowSize()
        {
            Vector2 contentSize = SceneBrowserContent.GetWindowSize();
            return new Vector2(contentSize.x, Mathf.Min(maxContentHeight, contentSize.y) + searchHeight + 12);
        }
    }
}