using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PropertyGroup
{
    public bool IsDrawByDefault;
    public List<TabGroup> tabs = new List<TabGroup>();

    // Index of the currently selected tab in this group
    public int selectedTabIndex = 0;

    public PropertyGroup(bool isDrawByDefault)
    {
        IsDrawByDefault = isDrawByDefault;
    }
}

public class TabGroup
{
    public string Name;
    public List<PropertyItem> items = new List<PropertyItem>();

    public FoldoutGroup currentFoldout;

    public TabGroup(string name = "MVsDefaultTab")
    {
        Name = name;
    }
}

public class PropertyItem { }

public class FoldoutGroup : PropertyItem
{
    public string Name;
    public List<PropertyField> fields = new List<PropertyField>();

    public FoldoutGroup(string name)
    {
        Name = name;
    }
}

public class PropertyField : PropertyItem
{
    public SerializedProperty property;

    public PropertyField() { }
    public PropertyField(SerializedProperty prop)
    {
        property = prop;
    }
}