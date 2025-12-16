using MVsToolkit.Dev;
using UnityEditor;
using UnityEngine;

public class Tst : MonoBehaviour
{
    [SerializeField, DrawInRect("DawRect")] int tst;

#if UNITY_EDITOR
    public void DrawRect(Rect rect)
    {
        EditorGUI.DrawRect(rect, Color.red);
    }
#endif
}