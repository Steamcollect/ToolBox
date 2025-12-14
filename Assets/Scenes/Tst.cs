using MVsToolkit.Demo;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

public class Tst : MonoBehaviour
{
    public InterfaceReference<IDemoInterface> reference;
    public InterfaceReference<IDemoInterface>[] references;

    [Button]
    public void TestA()
    {
        reference.Value.DemoMethod();
    }

    [Button]
    public void TestB()
    {
        foreach (var item in references)
        {
            item.Value.DemoMethod();
        }
    }
}