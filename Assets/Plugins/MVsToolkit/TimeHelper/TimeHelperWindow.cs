using System;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.TimeHelper
{
    /// <summary>
    /// Compact OnGUI editor window for timescale management.
    /// Features: slider, quick multiplier buttons, reset, and lock button.
    /// </summary>
    public class TimeHelperWindow: EditorWindow
    {
        private TimeHelperCore m_Core;
        
        private void OnEnable() => m_Core = new TimeHelperCore();
        private void OnDisable() => m_Core.Deconstruct();


        [MenuItem("MVsToolkit/TimeHelper/Timescale Manager %#t")]
        private static void ShowWindow()
        {
            TimeHelperWindow window = GetWindow<TimeHelperWindow>();
            window.Show();
        }
        
        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope()){}
            {
                DrawControlButtons();
                GUILayout.FlexibleSpace();
                DrawSliderSection();
            }

            // using (new EditorGUILayout.VerticalScope())
            // {
            //     // === Timescale Display ===
            //     DrawTimescaleDisplay(core);
            //
            //     GUILayout.Space(10);
            //
            //     // === Slider Section ===
            //     DrawSliderSection(core);
            //
            //     GUILayout.Space(10);
            //
            //     // === Quick Multiplier Buttons ===
            //     DrawQuickMultipliers(core);
            //
            //     GUILayout.Space(10);
            //
            //     // === Reset and Lock Buttons ===
            //     DrawControlButtons(core);
            //
            //     GUILayout.Space(10);
            //
            //     // === Preset Management ===
            //     DrawPresetSection(core);
            // }
        }

        private void DrawTimescaleDisplay(TimeHelperCore core)
        {
            string lockStatus = core.IsLocked ? " [LOCKED]" : "";
            string displayText = $"Timescale: {core.CurrentTimescale:F3}{lockStatus}";
            
            GUI.color = core.IsLocked ? Color.red : Color.white;
            GUILayout.Label(displayText, GUILayout.Height(20));
            GUI.color = Color.white;
        }

        private void DrawSliderSection()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                float newValue = GUILayout.HorizontalSlider(
                    m_Core.CurrentTimescale,
                    m_Core.MinTimescale,
                    m_Core.MaxTimescale
                );
                m_Core.SetTimescale(newValue);
            }
        }

        private void DrawQuickMultipliers(TimeHelperCore core)
        {
            GUILayout.Label("Quick Multipliers:", GUILayout.Height(18));
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("×0.5", GUILayout.Height(25)))
            {
                core.ApplyMultiplier(0.5f);
            }
            if (GUILayout.Button("×1", GUILayout.Height(25)))
            {
                core.SetTimescale(1f);
            }
            if (GUILayout.Button("×2", GUILayout.Height(25)))
            {
                core.ApplyMultiplier(2f);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawControlButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("",GUILayout.Width(20))){m_Core.Reset();}
                
                if (GUILayout.Button("",GUILayout.Width(20))){m_Core.ToggleLock();}
            }
        }

        // private void DrawPresetSection(TimeHelperCore core)
        // {
        //     GUILayout.Label("Presets:", GUILayout.Height(18));
        //     
        //     GUILayout.BeginHorizontal();
        //     
        //     // Save Preset Input
        //     _presetNameInput = GUILayout.TextField(_presetNameInput, GUILayout.Height(20), GUILayout.ExpandWidth(true));
        //     
        //     if (GUILayout.Button("Save", GUILayout.Height(20), GUILayout.Width(50)))
        //     {
        //         if (!string.IsNullOrEmpty(_presetNameInput))
        //         {
        //             string label = TimeHelperSettings.LoadResetLabel();
        //             TimeHelperSettings.CreatePreset(_presetNameInput, core.CurrentTimescale, label);
        //             _presetNameInput = "";
        //         }
        //     }
        //     
        //     GUILayout.EndHorizontal();
        //
        //     // Preset List Toggle
        //     if (GUILayout.Button(_showPresetMenu ? "Hide Presets" : "Show Presets", GUILayout.Height(20)))
        //     {
        //         _showPresetMenu = !_showPresetMenu;
        //     }
        //
        //     if (_showPresetMenu)
        //     {
        //         DrawPresetList();
        //     }
        // }
        //
        // private void DrawPresetList()
        // {
        //     GUILayout.BeginVertical(GUI.skin.box);
        //     _presetScrollPos = GUILayout.BeginScrollView(_presetScrollPos, GUILayout.Height(100));
        //
        //     string[] presets = TimeHelperSettings.GetAllPresetNames();
        //     if (presets.Length == 0)
        //     {
        //         GUILayout.Label("No presets saved", GUILayout.Height(20));
        //     }
        //     else
        //     {
        //         foreach (string presetName in presets)
        //         {
        //             GUILayout.BeginHorizontal(GUI.skin.box);
        //             
        //             if (GUILayout.Button($"Load: {presetName}", GUILayout.ExpandWidth(true), GUILayout.Height(20)))
        //             {
        //                 TimeHelperSettings.ApplyPreset(presetName, m_Core);
        //             }
        //             
        //             GUI.color = Color.red;
        //             if (GUILayout.Button("X", GUILayout.Width(25), GUILayout.Height(20)))
        //             {
        //                 TimeHelperSettings.DeletePreset(presetName);
        //             }
        //             GUI.color = Color.white;
        //             
        //             GUILayout.EndHorizontal();
        //         }
        //     }
        //
        //     GUILayout.EndScrollView();
        //     GUILayout.EndVertical();
        // }
    }
}

