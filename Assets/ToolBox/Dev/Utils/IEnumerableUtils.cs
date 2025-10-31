using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolBox.Utils
{
    public static class IEnumerableUtils
    {
        public static T GetRandom<T>(this IEnumerable<T> elems)
        {
            if (!elems.Any())
            {
                Debug.LogError("Try to get random elem from empty IEnumerable");
            }
            return elems.ElementAt(UnityEngine.Random.Range(0, elems.Count()));
        }
    }
}