using System.Collections.Generic;
using System.Reflection;
using MVsToolkit.Dev;
using MVsToolkit.Utils;
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
        propertyGroups.Add(new PropertyGroup(true));
        propertyGroups.GetLast().tabs.Add(new TabGroup());
    }

    private void ScanProperties(SerializedObject so, Object targetObj)
    {
        SerializedProperty iterator = so.GetIterator();
        if (!iterator.NextVisible(true))
            return;

        do
        {
            FieldInfo field = GetFieldRecursive(targetObj.GetType(), iterator.name);
            if(field == null) continue;

            TabAttribute tabAttr = field.GetCustomAttribute<TabAttribute>();
            CloseTabAttribute closeTabAttr = field.GetCustomAttribute<CloseTabAttribute>();

            if(closeTabAttr != null) // Close Tab
            {
                if (propertyGroups.GetLast().tabs.Count == 0)
                    propertyGroups.RemoveAt(propertyGroups.Count - 1);

                propertyGroups.Add(new PropertyGroup(true));
            }

            if(tabAttr != null) // Tab
            {
                if (propertyGroups.GetLast().IsDrawByDefault)
                    propertyGroups.Add(new PropertyGroup(false));

                propertyGroups.GetLast().tabs.Add(new TabGroup(tabAttr.tabName));
            }

            SerializedProperty prop = so.FindProperty(iterator.name);
            if (prop == null) continue;

            if (propertyGroups.GetLast().tabs.Count == 0)
                propertyGroups.GetLast().tabs.Add(new TabGroup());

            propertyGroups.GetLast().tabs.GetLast().items.Add(new PropertyField(prop));
        }
        while (iterator.NextVisible(false));
    }
    #endregion

    #region Helpers
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