using MVsToolkit.Dev;

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
            if (target.UnityObject is UnityEngine.GameObject go)
            {
                return go.CompareTag(Tag);
            }
            return false;
        }
    }
    
    public class TypeRule : IRenameRule
    {
        public System.Type Type;
        
        public bool Matches(IRenameTarget target, RenameContext ctx) => Type.IsAssignableFrom(target.UnityObject.GetType());
    }
}