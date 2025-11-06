using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ToolBox.Dev
{
    public class WatchDisplay : MonoBehaviour
    {
        private static List<(string name, Func<string> getter)> watchedVars = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            watchedVars.Clear();

            foreach (var mono in GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                var fields = mono.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var f in fields)
                {
                    if (Attribute.IsDefined(f, typeof(WatchAttribute)))
                    {
                        watchedVars.Add((
                            $"{mono.GetType().Name}.{f.Name}",
                            () => f.GetValue(mono)?.ToString() ?? "null"
                        ));
                    }
                }
            }

            // Crée un GameObject pour l’affichage
            var go = new GameObject("WatchDisplay");
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<WatchDisplay>();
            DontDestroyOnLoad(go);
        }

        void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            GUILayout.BeginArea(new Rect(10, Screen.height - 100, 400, 100));
            foreach (var (name, getter) in watchedVars)
            {
                GUILayout.Label($"{name} = {getter()}", style);
            }
            GUILayout.EndArea();
        }
    }
}