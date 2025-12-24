using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace MVsToolkit.Dev
{
    public class MVsInspectorPropertyGroup
    {
        public bool IsDrawByDefault;
        public List<MVsTabGroup> tabs = new List<MVsTabGroup>();

        // Index of the currently selected tab in this group
        public int selectedTabIndex = 0;

        public MVsInspectorPropertyGroup(bool isDrawByDefault)
        {
            IsDrawByDefault = isDrawByDefault;
        }
    }

    public class MVsTabGroup
    {
        public string Name;
        public List<MVsPropertyItem> items = new List<MVsPropertyItem>();

        public MVsFoldoutGroup currentFoldout;

        public MVsTabGroup(string name = "MVsDefaultTab")
        {
            Name = name;
        }
    }

    public class MVsPropertyItem { }

    public class MVsFoldoutGroup : MVsPropertyItem
    {
        public string Name;
        public List<MVsPropertyField> fields = new List<MVsPropertyField>();

        public MVsFoldoutGroup(string name)
        {
            Name = name;
        }
    }

    public class MVsPropertyField : MVsPropertyItem
    {
        public SerializedProperty property;

        public MVsPropertyField() { }
        public MVsPropertyField(SerializedProperty prop)
        {
            property = prop;
        }
    }

    public class MVsHandleData
    {
        public SerializedProperty property;
        public FieldInfo field;
        public HandleAttribute attribute;
    }
}