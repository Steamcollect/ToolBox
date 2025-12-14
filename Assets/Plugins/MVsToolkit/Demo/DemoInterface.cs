using UnityEngine;

namespace MVsToolkit.Demo
{
    public class DemoInterface : MonoBehaviour, IDemoInterface 
    {
        [SerializeField] string debugText;

        public void DemoMethod()
        {
            Debug.Log(debugText);
        }
    }
}