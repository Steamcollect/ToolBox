using UnityEngine;

namespace MVsToolkit.Dev
{
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float FMin, FMax;
        public readonly int IMin, IMax;

        public MinMaxRangeAttribute(int min, int max)
        {
            IMin = min;
            IMax = max;

            FMin = min;
            FMax = max;
        }

        public MinMaxRangeAttribute(float min, float max)
        {
            FMin = min;
            FMax = max;

            IMin = Mathf.RoundToInt(min);
            IMax = Mathf.RoundToInt(max);
        }
    }
}