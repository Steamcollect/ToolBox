using MVsToolkit.Dev;
using UnityEngine;

namespace MVsToolkit.Demo
{
    public class MVsToolkitInspectorDemo : MonoBehaviour
    {
        [Tab("Dropdowns")]
        [SerializeField] string[] dropdownValues;

        [Space(10)]
        [SerializeField, Dropdown(16, 32, 64, 128, 256, 512)] int dropdownDemo_1;
        [SerializeField, Dropdown("dropdownValues")] string dropdownDemo_2;

        [Tab("Toggle")]
        [SerializeField] bool showVariable;

        [SerializeField, ShowIf("showVariable", true)] float variableToShow;

        [SerializeField, HideIf("showVariable", false)] int variableToHide;

        [Tab("Fodlout")]
        [Foldout("Foldout")]
        [SerializeField] int foldoutIntA;
        [SerializeField] int foldoutIntB;
        [SerializeField] int foldoutIntC;

        [CloseTab, Tab("Class")]
        [SerializeField, Inline] InlineClass inlineClass;
        [SerializeField] InterfaceReference<IDemoInterface> demoInterface;

        [Tab("Handles")]
        [Handle] public Vector3 pointA;
        [Handle(Space.Self, HandleDrawType.Sphere, ColorPreset.Red)] public Vector3 pointB;
        [Handle(Space.World, HandleDrawType.Cube, ColorPreset.Cyan)] public Vector3 pointC;

        [Tab("Others")]
        [SerializeField, SceneName] string sceneName;
        [SerializeField, TagName] string tagName;

        [Space(10)]
        [SerializeField, Watch] int valueWatched;

        [Button]
        void DebugButtonA()
        {
            Debug.Log("Debug_A");
        }

        [Button("Debug_B")] // Direct string parameter
        void DebugButtonB(string debugText)
        {
            Debug.Log(debugText);
        }

        [Button("dropdownDemo_1")] // Refers to variable name
        void DebugButtonC(int debugValue)
        {
            Debug.Log(debugValue);
        }
    }
}