using UnityEngine;
using System.Collections.Generic;

public class PropertyGroup
{
    public List<TabGroup> tabs = new List<TabGroup>();
}

public class TabGroup
{
    public string Name;
    public bool IsInvisible;

    public List<PropertyItem> items = new List<PropertyItem>();

    public TabGroup(string name, bool isInvisible)
    {
        Name = name;
        IsInvisible = isInvisible;
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
    
}