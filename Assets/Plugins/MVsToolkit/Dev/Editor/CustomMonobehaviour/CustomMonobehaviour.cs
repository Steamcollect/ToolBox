using System.Collections.Generic;
using System.Reflection;
using MVsToolkit.Dev;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class CustomMonobehaviour : Editor
{
    public List<PropertyGroup> propertyGroups = new List<PropertyGroup>();

    private void OnEnable()
    {
        if (serializedObject == null)
            return;

        InitializeData();
        ScanProperties(serializedObject, target);
    }

    #region Initialization & Scanning
    void InitializeData()
    {
        propertyGroups.Clear();
        propertyGroups.Add(new PropertyGroup());
    }

    private void ScanProperties(SerializedObject so, Object targetObj)
    {
        SerializedProperty iterator = so.GetIterator();
        if (!iterator.NextVisible(true))
            return;

        do
        {
            FieldInfo field = GetFieldRecursive(targetObj.GetType(), iterator.name);
            if(field == null)
                continue;

            TabAttribute tabAttr = field.GetCustomAttribute<TabAttribute>();
            FoldoutAttribute foldoutAttr = field.GetCustomAttribute<FoldoutAttribute>();

            CloseTabAttribute closeTabAttr = field.GetCustomAttribute<CloseTabAttribute>();
            CloseFoldoutAttribute closeFoldoutAttr = field.GetCustomAttribute<CloseFoldoutAttribute>();

            HandleAttribute handleAttr = field.GetCustomAttribute<HandleAttribute>();

            if(closeTabAttr != null)
            {

            }
        } 
        while (iterator.NextVisible(false));
    }
    #endregion

    #region Helpers
    private object[] ResolveButtonsParameters(object[] rawParams, object target)
    {
        if (rawParams == null)
            return null;

        object[] resolved = new object[rawParams.Length];
        for (int i = 0; i < rawParams.Length; i++)
        {
            object param = rawParams[i];
            if (param is string s)
            {
                var field = target.GetType().GetField(s, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    resolved[i] = field.GetValue(target);
                    continue;
                }
            }
            resolved[i] = param;
        }
        return resolved;
    }
    private FieldInfo GetFieldRecursive(System.Type type, string fieldName)
    {
        while (type != null)
        {
            FieldInfo field = type.GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (field != null)
                return field;

            type = type.BaseType;
        }
        return null;
    }

    #endregion
}