
namespace MVsToolkit.BatchRename
{
    public interface IRenameTarget
    {
        string Name { get;}
        string Path { get;}
        UnityEngine.Object UnityObject { get;}

        void SetName(string newName);
    }
}