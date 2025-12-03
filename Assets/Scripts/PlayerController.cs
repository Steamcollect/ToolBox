using MVsToolkit.Dev;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Handle(Space.Self)] Vector3 handle1;
    [SerializeField, Handle] Vector2 handle2;
}