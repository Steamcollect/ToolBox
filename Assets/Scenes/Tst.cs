using MVsToolkit.Demo;
using MVsToolkit.Dev;
using UnityEngine;

public class Tst : MVsToolkitInspectorDemo
{
    // Tout par défaut : Local, Default, White
    [Handle]
    public Vector3 pointD;

    // Handle local, forme Sphere, couleur Rouge
    [Handle(Space.Self, HandleDrawType.Sphere, ColorPreset.Red)]
    public Vector3 pointE;

    // Handle global, forme Cube, couleur Cyan
    [Handle(Space.World, HandleDrawType.Cube, ColorPreset.Cyan)]
    public Vector3 pointF;
}