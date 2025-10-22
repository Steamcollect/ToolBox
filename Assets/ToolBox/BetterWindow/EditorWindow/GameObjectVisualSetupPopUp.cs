using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GameObjectVisualSetupPopUp : PopupWindowContent
{
    Color goColor;

    public override void OnGUI(Rect rect)
    {
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select color");
        Color color = EditorGUILayout.ColorField(goColor);
        GUILayout.EndHorizontal();
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 200);
    }
}