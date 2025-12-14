#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MVsToolkit.BetterInterface
{
    public class SceneBrowserPopUp : PopupWindowContent
    {
        int maxHeight = 300;
        string searchTxt;

        public override void OnOpen()
        {
            SceneBrowserContent.OnOpen();
        }
        public override void OnClose()
        {
            SceneBrowserContent.OnClose();
        }

        public override void OnGUI(Rect rect)
        {
            searchTxt = EditorGUILayout.TextField(searchTxt, searchTxt);
            SceneBrowserContent.OnGUI(rect, searchTxt);
        }

        public override Vector2 GetWindowSize()
        {
            return SceneBrowserContent.GetWindowSize(maxHeight);
        }
    }
}
#endif