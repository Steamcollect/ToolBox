using System;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace MVsToolkit.Utils
{
    public static class CoroutineUtils
    {
        public static void Delay(this MonoBehaviour hook, Action ev, YieldInstruction yieldInstruction)
        {
            IEnumerator DelayCoroutine()
            {
                yield return yieldInstruction;
                ev?.Invoke();
            }

            hook.StartCoroutine(DelayCoroutine());
        }

        public static void Delay(this MonoBehaviour hook, Action ev, float time)
        {
            IEnumerator DelayCoroutine()
            {
                yield return new WaitForSeconds(time);
                ev?.Invoke();
            }

            hook.StartCoroutine(DelayCoroutine());
        }

        public static void Delay(this MonoBehaviour hook, Action ev, IEnumerator coroutine)
        {
            IEnumerator DelayCoroutine()
            {
                yield return coroutine;
                ev?.Invoke();
            }

            hook.StartCoroutine(DelayCoroutine());
        }
    }
}