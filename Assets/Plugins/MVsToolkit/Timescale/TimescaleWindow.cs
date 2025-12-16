using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace MVsToolkit.BetterInterface
{
    public class TimescaleWindow : EditorWindow
    {
        private const float k_SliderMin = 0f;
        private const float k_SliderMax = 2f;
        private const string k_PrefsTimescale = "MVsToolkit.Timescale.Value";
        private const string k_PrefsFixedDeltaTime = "MVsToolkit.Timescale.FixedDeltaTime";
        private const string k_PrefsPresets = "MVsToolkit.Timescale.Presets";

        private static float s_BaseFixedDeltaTime = 0.02f;
        private static float s_BaseMaximumDeltaTime = 0.33333334f;
        private static float s_BaseMaximumParticleDeltaTime = 0.03f;
        
        private static float[] s_Presets = { 0.5f, 1f, 2f };
        private float m_CurrentTimescaleInput = 1f;

        [MenuItem("Tools/MVsToolkit/Timescale Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<TimescaleWindow>("TS");
            window.ShowTab();
        }

        [Shortcut("MVsToolkit/Timescale 0.5x", KeyCode.Comma, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        private static void SetTimescale050X() => SetPresetTimescale(0.5f);

        [Shortcut("MVsToolkit/Timescale 1x", KeyCode.Period, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        private static void SetTimescale1X() => SetPresetTimescale(1f);

        [Shortcut("MVsToolkit/Timescale 2x", KeyCode.Slash, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        private static void SetTimescale2X() => SetPresetTimescale(2f);

        private static void SetPresetTimescale(float scale)
        {
            ApplyTimeScaleInternal(scale);
            SaveTimescaleToEditorPrefs();
            var window = GetWindow<TimescaleWindow>("TS");
            window.Repaint();
        }

        private void OnEnable()
        {
            LoadPresetsFromEditorPrefs();
            LoadTimescaleFromEditorPrefs();
            CaptureBaseTimes();
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
            EditorApplication.quitting += HandleEditorQuitting;
        }

        private void OnDisable()
        {
            SaveTimescaleToEditorPrefs();
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.quitting -= HandleEditorQuitting;
        }

        private void OnGUI()
        {
            float currentScale = Time.timeScale;
            Color originalColor = GUI.color;

            // Déterminer la couleur du label en fonction du timescale
            GUI.color = GetTimescaleColor(currentScale);
            EditorGUILayout.LabelField("Timescale", EditorStyles.boldLabel);
            GUI.color = originalColor;

            EditorGUILayout.Space(4f);

            // Champ numérique pour entrée directe
            EditorGUI.BeginChangeCheck();
            m_CurrentTimescaleInput = EditorGUILayout.FloatField("TS:", currentScale, GUILayout.Height(20f));
            if (EditorGUI.EndChangeCheck())
            {
                m_CurrentTimescaleInput = Mathf.Clamp(m_CurrentTimescaleInput, k_SliderMin, k_SliderMax);
                SetTimeScale(m_CurrentTimescaleInput);
            }

            // Slider synchronisé
            EditorGUI.BeginChangeCheck();
            float sliderValue = EditorGUILayout.Slider(currentScale, k_SliderMin, k_SliderMax, GUILayout.Height(18f));
            if (EditorGUI.EndChangeCheck())
            {
                SetTimeScale(sliderValue);
                m_CurrentTimescaleInput = sliderValue;
            }

            EditorGUILayout.Space(6f);

            // Boutons presets configurables dans une rangée compacte
            if (s_Presets.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Presets:", GUILayout.Width(50f));
                for (int i = 0; i < s_Presets.Length; i++)
                {
                    string label = $"{s_Presets[i]}x";
                    if (GUILayout.Button(label, GUILayout.Height(18f)))
                    {
                        SetTimeScale(s_Presets[i]);
                        m_CurrentTimescaleInput = s_Presets[i];
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(4f);

            // Boutons d'action rapides
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("×2", GUILayout.Height(18f))) SetTimeScale(currentScale * 2f);
            if (GUILayout.Button("÷2", GUILayout.Height(18f))) SetTimeScale(currentScale * 0.5f);
            if (GUILayout.Button("Reset", GUILayout.Height(18f))) SetTimeScale(1f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6f);

            // Affichage des valeurs de temps
            EditorGUILayout.LabelField("Base Fixed DeltaTime:", s_BaseFixedDeltaTime.ToString("F5"));
            EditorGUILayout.LabelField("Current Fixed DeltaTime:", Time.fixedDeltaTime.ToString("F5"));
        }

        /// <summary>
        /// Retourne une couleur en fonction du timescale pour feedback visuel
        /// </summary>
        private static Color GetTimescaleColor(float scale)
        {
            if (Mathf.Abs(scale - 1f) < 0.01f) return Color.green;      // Vert si 1.0x
            if (scale < 1f) return new Color(1f, 0.647f, 0f);           // Orange si ralenti
            return new Color(0.2f, 0.6f, 1f);                            // Bleu si accéléré
        }

        private void SetTimeScale(float newScale)
        {
            ApplyTimeScaleInternal(newScale);
            SaveTimescaleToEditorPrefs();
            Repaint();
        }

        private static void ApplyTimeScaleInternal(float newScale)
        {
            newScale = Mathf.Clamp(newScale, k_SliderMin, k_SliderMax);
            Time.timeScale = newScale;
            Time.fixedDeltaTime = s_BaseFixedDeltaTime * newScale;
            Time.maximumDeltaTime = s_BaseMaximumDeltaTime * newScale;
            Time.maximumParticleDeltaTime = s_BaseMaximumParticleDeltaTime * newScale;
        }

        private static void ResetTimeScale()
        {
            ApplyTimeScaleInternal(1f);
        }

        private static void CaptureBaseTimes()
        {
            float safeScale = Mathf.Max(Time.timeScale, 0.0001f);
            s_BaseFixedDeltaTime = Time.fixedDeltaTime / safeScale;
            s_BaseMaximumDeltaTime = Time.maximumDeltaTime / safeScale;
            s_BaseMaximumParticleDeltaTime = Time.maximumParticleDeltaTime / safeScale;
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange change)
        {
            if (change is not (PlayModeStateChange.EnteredEditMode or PlayModeStateChange.EnteredPlayMode)) return;
            CaptureBaseTimes();
            ResetTimeScale();
        }

        private static void HandleEditorQuitting()
        {
            ResetTimeScale();
        }

        /// <summary>
        /// Charge les presets depuis EditorPrefs ou utilise les valeurs par défaut
        /// </summary>
        private static void LoadPresetsFromEditorPrefs()
        {
            string presetsJson = EditorPrefs.GetString(k_PrefsPresets, "");
            if (string.IsNullOrEmpty(presetsJson))
            {
                s_Presets = new[] { 0.5f, 1f, 2f };
            }
            else
            {
                try
                {
                    // Parsing simple : "0.5,1,2"
                    string[] parts = presetsJson.Split(',');
                    var presets = new List<float>();
                    foreach (string part in parts)
                    {
                        if (float.TryParse(part.Trim(), out float value))
                        {
                            presets.Add(Mathf.Clamp(value, k_SliderMin, k_SliderMax));
                        }
                    }
                    s_Presets = presets.Count > 0 ? presets.ToArray() : new[] { 0.5f, 1f, 2f };
                }
                catch
                {
                    s_Presets = new[] { 0.5f, 1f, 2f };
                }
            }
        }

        /// <summary>
        /// Charge le timescale et fixedDeltaTime depuis EditorPrefs
        /// </summary>
        private static void LoadTimescaleFromEditorPrefs()
        {
            float savedTimescale = EditorPrefs.GetFloat(k_PrefsTimescale, 1f);
            float savedFixedDeltaTime = EditorPrefs.GetFloat(k_PrefsFixedDeltaTime, 0.02f);

            // Restaurer les valeurs
            s_BaseFixedDeltaTime = savedFixedDeltaTime;
            ApplyTimeScaleInternal(savedTimescale);
        }

        /// <summary>
        /// Sauvegarde le timescale et fixedDeltaTime dans EditorPrefs
        /// </summary>
        private static void SaveTimescaleToEditorPrefs()
        {
            EditorPrefs.SetFloat(k_PrefsTimescale, Time.timeScale);
            EditorPrefs.SetFloat(k_PrefsFixedDeltaTime, s_BaseFixedDeltaTime);
        }
    }
}
