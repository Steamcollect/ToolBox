using MVsToolkit.Dev;
using System.Collections;
using UnityEngine;

public class Tst : MonoBehaviour
{
    [SerializeField] MVsPool<GameObject> bulletPool;

    void Start()
    {
        bulletPool.Init();
    }

    [Button]
    void Test()
    {
        StartCoroutine(TestCor());
    }

    IEnumerator TestCor()
    {
        bulletPool.Get(out GameObject c);
        yield return new WaitForSeconds(1f);
        bulletPool.Release(c);
    }
}