using MVsToolkit.Dev;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string printTest;
    public string warningTest;
    public string errorTest;

    [Button]
    void Print()
    {
        Debug.Log(printTest);
    }
   
    [Button]
    void Warning()
    {
        Debug.Log(warningTest);
    }
    
    [Button]
    void Error()
    {
        Debug.Log(errorTest);
    }
}