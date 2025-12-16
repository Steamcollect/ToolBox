using MVsToolkit.Dev;
using UnityEditor;
using UnityEngine;

public class Tst : MonoBehaviour
{
    [SerializeField, DrawInRect("DawRect")] int tst;

    public void DrawRect(Rect rect)
    {
        EditorGUI.DrawRect(rect, Color.red);
    }
}