using MVsToolkit.Dev;
using UnityEngine;

namespace MVsToolkit.Demo
{
    public class InspectorDemo : MonoBehaviour
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

        [Tab("Foldout")]
        [Foldout("Foldout")]
        [SerializeField] float foldoutA;
        [SerializeField] float foldoutB;
        [SerializeField] float foldoutC;

        [Tab("Class")]
        [SerializeField, Inline] InlineClass inlineClass;
        [SerializeField] InterfaceReference<IDemoInterface> demoInterface;

        [Tab("Others")]
        [SerializeField, SceneName] string sceneName;
        [SerializeField, TagName] string tagName;
        [SerializeField, Watch] int valueWatched;

        [Button]
        void DebugButtonA()
        {
            Debug.Log("Debug_A");
        }

        [Button("Debug_B")]
        void DebugButtonB(string debugText)
        {
            Debug.Log(debugText);
        }

        [Button(123456)]
        void DebugButtonC(int debugValue)
        {
            Debug.Log(debugValue);
        }
    }
}