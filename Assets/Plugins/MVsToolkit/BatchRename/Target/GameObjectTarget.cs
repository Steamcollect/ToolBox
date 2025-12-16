using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public class GameObjectTarget : IRenameTarget
    {
        private readonly GameObject m_GameObject;

        public GameObjectTarget(GameObject gameObject)
        {
            m_GameObject = gameObject;
        }

        public string Name => m_GameObject.name;
        public string Path => HierarchyHelper.GetHierarchyPath(m_GameObject);
        public Object UnityObject => m_GameObject;

        public void SetName(string newName)
        {
            m_GameObject.name = newName;
        }

        public bool IsValidTarget()
        {
            return m_GameObject != null;
        }
    }
}