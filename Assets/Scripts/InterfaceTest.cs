using MVsToolkit.Dev;
using UnityEngine;

public class InterfaceTest : MonoBehaviour
{
    [SerializeField] InterfaceReference<ISerializedInterface> IInterface;
}

public interface ISerializedInterface
{
    void Start();
}