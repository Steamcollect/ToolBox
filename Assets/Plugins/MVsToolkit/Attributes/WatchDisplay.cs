using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// MonoBehaviour component that provides runtime display for fields marked with [Watch] attribute.
    /// <para>
    /// Automatically created at runtime to show watched variable values on screen during Play Mode.
    /// </para>
    /// <para>
    /// This class should not be added manually to GameObjects - it is automatically instantiated.
    /// </para>
    /// <para>
    /// Displays all fields marked with [Watch] in the bottom-left corner of the screen.
    /// </para>
    /// </summary>
    public class WatchDisplay : MonoBehaviour
    {
        private static List<(string name, Func<string> getter)> watchedVars = new();

        /// <summary>
        /// Initializes the watch display system after scene load.
        /// Scans all MonoBehaviours for fields marked with [Watch] attribute.
        /// </summary>
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

            // Create a GameObject for display
            var go = new GameObject("WatchDisplay");
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<WatchDisplay>();
            DontDestroyOnLoad(go);
        }

        /// <summary>
        /// Renders watched variable values on screen using Unity's immediate mode GUI.
        /// </summary>
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