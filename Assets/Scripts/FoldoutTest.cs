using ToolBox.Dev;
using UnityEngine;

public class FoldoutTest : MonoBehaviour
{
    [Foldout("Main Stats")]
    public int Health;
    public int damage;

    [Foldout("Test 2")]
    public string test;
    [Dropdown(1, 2, 3, 1, 4, 55, 8, 5)] public int haha;

    [Button]
    void ButtonTest()
    {
        print(haha);
    }
}