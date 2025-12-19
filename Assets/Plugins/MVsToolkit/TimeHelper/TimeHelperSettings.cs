using System;
using UnityEngine;
using System.Collections.Generic;

namespace MVsToolkit.TimeHelper
{
    /// <summary>
    /// Manages timescale settings, presets, and user preferences.
    /// Handles PlayerPrefs serialization and automatic saves.
    /// </summary>
    public static class TimeHelperSettings
    {
        private const string k_PrefsTimescaleKey = "MVsToolkit_TimeHelper_Timescale";
        private const string k_PrefsLockStateKey = "MVsToolkit_TimeHelper_TimescaleLocked";
        private const string k_PrefsResetLabelKey = "MVsToolkit_ResetLabel";
        private const string k_PrefsPresetCountKey = "MVsToolkit_PresetCount";
        private const string k_PrefsPresetPrefix = "MVsToolkit_Preset_";
        private const string k_PrefsActivePresetKey = "MVsToolkit_ActivePreset";

        private const float k_DefaultTimescale = 1f;
        private const string k_DefaultResetLabel = "Reset";

        private static readonly Dictionary<string, PresetData> s_LoadedPresets;

        static TimeHelperSettings()
        {
            s_LoadedPresets = new Dictionary<string, PresetData>();
        }

        #region Timescale Management

        /// <summary>
        /// Saves the current timescale value.
        /// </summary>
        public static void SaveTimescale(float value)
        {
            PlayerPrefs.SetFloat(k_PrefsTimescaleKey, value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads the saved timescale value.
        /// </summary>
        public static float LoadTimescale()
        {
            return PlayerPrefs.GetFloat(k_PrefsTimescaleKey, k_DefaultTimescale);
        }

        #endregion

        #region Lock State

        /// <summary>
        /// Saves the lock state.
        /// </summary>
        public static void SaveLockState(bool locked)
        {
            PlayerPrefs.SetInt(k_PrefsLockStateKey, locked ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads the saved lock state.
        /// </summary>
        public static bool LoadLockState()
        {
            return PlayerPrefs.GetInt(k_PrefsLockStateKey, 0) == 1;
        }

        #endregion

        #region Reset Label

        /// <summary>
        /// Saves the reset button label.
        /// </summary>
        public static void SaveResetLabel(string label)
        {
            PlayerPrefs.SetString(k_PrefsResetLabelKey, label);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads the saved reset button label.
        /// </summary>
        public static string LoadResetLabel()
        {
            return PlayerPrefs.GetString(k_PrefsResetLabelKey, k_DefaultResetLabel);
        }

        #endregion

        #region Preset Management

        /// <summary>
        /// Creates and saves a new preset.
        /// </summary>
        public static void CreatePreset(string presetName, float timescaleValue, string resetLabel, bool[] buttonStates = null)
        {
            PresetData preset = new PresetData
            {
                timescaleValue = timescaleValue,
                resetLabel = resetLabel,
                buttonStates = buttonStates ?? Array.Empty<bool>()
            };

            s_LoadedPresets[presetName] = preset;
            SavePresetToPlayerPrefs(presetName, preset);
        }

        /// <summary>
        /// Loads a preset by name and applies it.
        /// </summary>
        public static PresetData LoadPreset(string presetName)
        {
            if (s_LoadedPresets.TryGetValue(presetName, out var preset))
            {
                return preset;
            }

            preset = LoadPresetFromPlayerPrefs(presetName);
            if (preset != null)
            {
                s_LoadedPresets[presetName] = preset;
            }
            return preset;
        }

        /// <summary>
        /// Applies a preset to the core.
        /// </summary>
        public static void ApplyPreset(string presetName, TimeHelperCore core)
        {
            PresetData preset = LoadPreset(presetName);
            if (preset != null)
            {
                core.SetTimescale(preset.timescaleValue);
                SaveResetLabel(preset.resetLabel);
                PlayerPrefs.SetString(k_PrefsActivePresetKey, presetName);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Deletes a preset.
        /// </summary>
        public static void DeletePreset(string presetName)
        {
            s_LoadedPresets.Remove(presetName);
            PlayerPrefs.DeleteKey(k_PrefsPresetPrefix + presetName);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets all available preset names.
        /// </summary>
        public static string[] GetAllPresetNames()
        {
            return new List<string>(s_LoadedPresets.Keys).ToArray();
        }

        /// <summary>
        /// Gets the active preset name.
        /// </summary>
        public static string GetActivePresetName()
        {
            return PlayerPrefs.GetString(k_PrefsActivePresetKey, "");
        }

        #endregion

        #region PlayerPrefs Serialization

        private static void SavePresetToPlayerPrefs(string presetName, PresetData preset)
        {
            string key = k_PrefsPresetPrefix + presetName;
            string json = JsonUtility.ToJson(preset);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        private static PresetData LoadPresetFromPlayerPrefs(string presetName)
        {
            string key = k_PrefsPresetPrefix + presetName;
            if (!PlayerPrefs.HasKey(key))
                return null;

            string json = PlayerPrefs.GetString(key, "");
            try
            {
                return JsonUtility.FromJson<PresetData>(json);
            }
            catch
            {
                Debug.LogWarning($"Failed to deserialize preset: {presetName}");
                return null;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Clears all saved settings.
        /// </summary>
        public static void ClearAllSettings()
        {
            PlayerPrefs.DeleteKey(k_PrefsTimescaleKey);
            PlayerPrefs.DeleteKey(k_PrefsLockStateKey);
            PlayerPrefs.DeleteKey(k_PrefsResetLabelKey);
            PlayerPrefs.DeleteKey(k_PrefsActivePresetKey);
            s_LoadedPresets.Clear();
            PlayerPrefs.Save();
        }

        #endregion

        /// <summary>
        /// Data structure for preset information.
        /// </summary>
        [System.Serializable]
        public class PresetData
        {
            public float timescaleValue;
            public string resetLabel;
            public bool[] buttonStates;
        }
    }
}
