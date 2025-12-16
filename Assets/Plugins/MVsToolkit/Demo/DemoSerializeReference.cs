
using UnityEngine;

namespace MVsToolkit.Demo
{
    public class DemoSerializeReference : MonoBehaviour
    {

        [Header("Settings")]

        [SerializeReference, Dev.SerializeReferenceDrawer] private BaseClass[] m_DemoClassPrivateArray;
        [SerializeReference, Dev.SerializeReferenceDrawer] public BaseClass DemoClass;
        [SerializeReference, Dev.SerializeReferenceDrawer] private BaseClass m_DemoClassPrivate;
        
        [SerializeReference, Dev.SerializeReferenceDrawer] public IInterfaceExample DemoInterfaceImpl;
    }


    [System.Serializable]
    public struct BaseStruct
    {
        public float SomeValue;
    }
    
    [System.Serializable]
    public abstract class BaseClass{}
    
    [System.Serializable]
    public class ChildClassA : BaseClass {}

    [System.Serializable]
    public class ChildClassB : BaseClass
    {
        public float SomeValue;
    }
    
    public interface IInterfaceExample {  }
    
    [System.Serializable]
    public class InterfaceImplA : IInterfaceExample {  }

    [System.Serializable]
    public class InterfaceImplB : IInterfaceExample
    {
        public float SomeValue;
    }
    
}