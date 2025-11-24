using UnityEngine;

public class HandleAttribute : PropertyAttribute 
{
    public readonly TransformLocationType HandleType;

    public HandleAttribute(TransformLocationType handleType = TransformLocationType.Global) 
    {
        HandleType = handleType;
    }
}

public enum TransformLocationType
{
    Global,
    Local,
}