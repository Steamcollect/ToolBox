using MVsToolkit.Dev;
using UnityEngine;

public class DropdownTest : MonoBehaviour
{
    public Color color;

    public string[] fileTypes;
    public int[] fileSizes;
    public float[] fileCompressions;

    [Space(10)]
    [Dropdown("fileTypes")] public Vector2 fileTypess;
    [Space(5)]
    [Dropdown("fileTypes")] public string fileType;
    [Dropdown("fileSizes")] public int fileSize;
    [Dropdown("fileCompressions")] public float fileCompression;
    
    [Space(10)]
    [Dropdown("Ababa", "Hohoho", "Hihihi")] public string test1;
    [Dropdown(16, 32, 64, 128)] public int test2;
    [Dropdown(16.5, 32.1235, 64f, .45)] public float test3;

    [Button]
    void Test()
    {
        print(fileType);
        print(fileSize);
        print(fileCompression);
        print(test1);
        print(test2);
        print(test3);
    }
}