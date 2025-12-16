using UnityEngine;

namespace MVsToolkit.Dev
{
    public class DrawInRectAttribute : PropertyAttribute
    {
        public string methodName;
        public float height;

        public DrawInRectAttribute(string methodName, float height = 30f)
        {
            this.methodName = methodName;
            this.height = height;
        }
    }
}