using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class TestUtils 
{
    /// <summary>
    /// Load asset by name, need to be instantiating if needed, work only in editor mode
    /// </summary>
    /// <typeparam name="T">Type of object returned</typeparam>
    /// <returns>Object Loaded</returns>
    public static T LoadAssetEditorMode<T>(string assetName) where T : Object
    {
        string[] guids = AssetDatabase.FindAssets(assetName);
        if (guids.Length > 0)
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
        return null;
    }

    /// <summary>
    /// Inject a value in a object with a specific property
    /// </summary>
    /// <param name="obj">Object target</param>
    /// <param name="propertyName">Name of the property, same name needed</param>
    /// <param name="value"></param>
    /// <typeparam name="T">Type of the value injected</typeparam>
    public static void InjectValue<T>(this object obj, string propertyName,T value)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.SetField;
        Type type = obj.GetType();
        FieldInfo field = type.GetField(propertyName, flags);
        field.SetValue(obj, value);
    }

    public static T GetFieldValue<T>(this object obj, string propertyName)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty;
        Type type = obj.GetType();
        FieldInfo field = type.GetField(propertyName, flags);
        return (T)field.GetValue(obj);
    }

    /// <summary>
    /// Create a MonoBehaviour and disable the Hooker (GameObject)
    /// </summary>
    /// <typeparam name="T">Type of the MonoBehaviour</typeparam>
    /// <returns>Instance of the MonoBehaviour in state active false</returns>
    public static T CreateMB<T>() where T : MonoBehaviour
    {
        GameObject hook = new GameObject();
        hook.SetActive(false);
        return hook.AddComponent<T>();
    }
}
