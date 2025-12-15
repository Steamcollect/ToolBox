using MVsToolkit.Demo;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

public class Tst : MonoBehaviour
{
    [SerializeField] MVsPool<Camera> bulletPool;

    [Button]
    public void TestA()
    {
        bulletPool.Get();
    }
}