using System;
using MVsToolkit.Dev;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public interface IRenameRule
    {
        bool Matches(IRenameTarget target, RenameContext ctx);
    }


    public class TagRule : IRenameRule
    {
        [TagName] public string Tag;

        public bool Matches(IRenameTarget target, RenameContext ctx)
        {
            if (target.UnityObject is GameObject go) return go.CompareTag(Tag);
            return false;
        }
    }

    public class TypeRule : IRenameRule
    {
        public Type Type;

        public bool Matches(IRenameTarget target, RenameContext ctx)
        {
            return Type.IsAssignableFrom(target.UnityObject.GetType());
        }
    }
}