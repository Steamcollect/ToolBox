using MVsToolkit.Dev;
using UnityEngine;

public class Tst : MonoBehaviour
{
    // Tout par défaut : Local, Default, White
    [Handle]
    public Vector3 pointA;

    // Handle local, forme Sphere, couleur Rouge
    [Handle(TransformLocationType.Local, HandleDrawType.Sphere, ColorPreset.Red)]
    public Vector3 pointB;

    // Handle global, forme Cube, couleur Cyan
    [Handle(TransformLocationType.Global, HandleDrawType.Cube, ColorPreset.Cyan)]
    public Vector3 pointC;

}