using UnityEngine;

public class InterfaceTest : MonoBehaviour
{
    [SerializedInterface] ISerializedInterface IInterface;
}

public interface ISerializedInterface
{
    void Start();
}