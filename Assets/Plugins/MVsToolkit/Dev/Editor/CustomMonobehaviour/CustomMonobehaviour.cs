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
            // Determine root field name from propertyPath (handles nested properties and arrays)
            string path = iterator.propertyPath ?? iterator.name;
            string rootName = path;

            int arrayIndex = path.IndexOf(".Array.data");
            if (arrayIndex >= 0)
                rootName = path.Substring(0, arrayIndex);
            else
            {
                int dot = path.IndexOf('.');
                if (dot >= 0)
                    rootName = path.Substring(0, dot);
            }

            FieldInfo field = GetFieldRecursive(targetObj.GetType(), rootName);

            // Get attributes only if field found. We still want to include properties like m_Script
            TabAttribute tabAttr = field?.GetCustomAttribute<TabAttribute>();
            CloseTabAttribute closeTabAttr = field?.GetCustomAttribute<CloseTabAttribute>();

            if (closeTabAttr != null) // Close Tab
            {
                // Safe removal: ensure there's at least one group
                if (propertyGroups.Count > 0 && propertyGroups.GetLast().tabs.Count == 0)
                    propertyGroups.RemoveAt(propertyGroups.Count - 1);

                propertyGroups.Add(new PropertyGroup(true));
            }

            if (tabAttr != null) // Tab
            {
                if (propertyGroups.Count == 0 || propertyGroups.GetLast().IsDrawByDefault)
                    propertyGroups.Add(new PropertyGroup(false));

                propertyGroups.GetLast().tabs.Add(new TabGroup(tabAttr.tabName));
            }

            // Use a copy of the iterator so stored SerializedProperty isn't invalidated by iteration
            SerializedProperty prop = iterator.Copy();
            if (prop == null)
                continue;

            // Ensure we have at least one property group and one tab
            if (propertyGroups.Count == 0)
                InitializeData();

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