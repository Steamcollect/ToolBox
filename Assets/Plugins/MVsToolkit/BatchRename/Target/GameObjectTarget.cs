namespace MVsToolkit.BatchRename
{
    public class GameObjectTarget : IRenameTarget
    {
        public string Name => m_GameObject.name; 
        public string Path => HierarchyHelper.GetHierarchyPath(m_GameObject);
        public UnityEngine.Object UnityObject => m_GameObject;
        
        private readonly UnityEngine.GameObject m_GameObject;

        public GameObjectTarget(UnityEngine.GameObject gameObject) => this.m_GameObject = gameObject;
        
        public void SetName(string newName)
        {
            m_GameObject.name = newName;
        }
    }
}