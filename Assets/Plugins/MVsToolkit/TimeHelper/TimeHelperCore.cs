using System;
using UnityEngine;

namespace MVsToolkit.TimeHelper
{
    /// <summary>
    /// Core timescale management system.
    /// Handles all timescale modifications, locking mechanism, and synchronized state.
    /// Single source of truth for timescale operations.
    /// </summary>
    public class TimeHelperCore
    {
        #region Fields

        private bool m_IsLocked = false;
        private float m_CurrentTimescale = 1f;
        
        private float m_MinTimescale = 0f;
        private float m_MaxTimescale = 2f;

        #endregion

        #region Properties

        public float CurrentTimescale => m_CurrentTimescale;
        public bool IsLocked => m_IsLocked;
        public float MinTimescale => m_MinTimescale;
        public float MaxTimescale => m_MaxTimescale;

        #endregion
        
        #region API

        /// <summary>
        /// Sets the timescale if not locked.
        /// </summary>
        public void SetTimescale(float value)
        {
            if (m_IsLocked) return;

            value = Mathf.Clamp(value, m_MinTimescale, m_MaxTimescale);
            if (Mathf.Approximately(m_CurrentTimescale, value)) return;

            m_CurrentTimescale = value;
            Time.timeScale = value;
        }

        /// <summary>
        /// Applies a multiplier to the current timescale.
        /// </summary>
        public void ApplyMultiplier(float multiplier)
        {
            float newValue = m_CurrentTimescale * multiplier;
            SetTimescale(newValue);
        }

        /// <summary>
        /// Resets timescale to 1.
        /// </summary>
        public void Reset()
        {
            SetTimescale(1f);
        }

        /// <summary>
        /// Toggles lock state.
        /// </summary>
        public void ToggleLock() => m_IsLocked = !m_IsLocked;

        #endregion
        
        #region Lifecycle

        public TimeHelperCore()
        {
            LoadSavedSettings();
        }
        
        public void Deconstruct()
        {
            SaveCurrentSettings();
        }
        
        private void LoadSavedSettings()
        {
            m_CurrentTimescale = TimeHelperSettings.LoadTimescale();
            m_IsLocked = TimeHelperSettings.LoadLockState();
            Time.timeScale = m_CurrentTimescale;
        }

        private void SaveCurrentSettings()
        {
            TimeHelperSettings.SaveTimescale(m_CurrentTimescale);
            TimeHelperSettings.SaveLockState(m_IsLocked);
        }

        #endregion
    }
}
