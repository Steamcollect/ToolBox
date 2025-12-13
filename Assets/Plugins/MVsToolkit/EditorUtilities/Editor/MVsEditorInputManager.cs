#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define USE_NEW_INPUT_SYSTEM
#endif

using UnityEngine;
using System.Collections.Generic;

#if USE_NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace MVsToolkit.BetterInterface
{
    public static class MVsEditorInputManager
    {
        public static KeyCode[] GetKeys()
        {
            List<KeyCode> keys = new List<KeyCode>();

#if USE_NEW_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard == null)
                return keys.ToArray();

            foreach (KeyControl key in keyboard.allKeys)
            {
                if (key == null)
                    continue;

                if (key.isPressed)
                {
                    if (System.Enum.TryParse<KeyCode>(key.name, true, out var kc))
                        keys.Add(kc);
                }
            }
#else
        // Old input system
        if (Input.anyKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                    keys.Add(key);
            }
        }
#endif

            return keys.ToArray();
        }

        public static bool Ctrl =>
#if USE_NEW_INPUT_SYSTEM
            (Keyboard.current?.leftCtrlKey?.isPressed ?? false) ||
            (Keyboard.current?.rightCtrlKey?.isPressed ?? false);
#else
        Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
#endif

        public static bool Shift =>
#if USE_NEW_INPUT_SYSTEM
            (Keyboard.current?.leftShiftKey?.isPressed ?? false) ||
            (Keyboard.current?.rightShiftKey?.isPressed ?? false);
#else
        Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif

        public static bool Alt =>
#if USE_NEW_INPUT_SYSTEM
            (Keyboard.current?.leftAltKey?.isPressed ?? false) ||
            (Keyboard.current?.rightAltKey?.isPressed ?? false);
#else
        Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
#endif

        public static bool LeftMouse
        {
            get
            {
#if USE_NEW_INPUT_SYSTEM
                var mouse = Mouse.current;
                return mouse != null && mouse.leftButton.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
            }
        }

        public static bool RightMouse
        {
            get
            {
#if USE_NEW_INPUT_SYSTEM
                var mouse = Mouse.current;
                return mouse != null && mouse.rightButton.isPressed;
#else
        return Input.GetMouseButton(1);
#endif
            }
        }

        public static bool MiddleMouse
        {
            get
            {
#if USE_NEW_INPUT_SYSTEM
                var mouse = Mouse.current;
                return mouse != null && mouse.middleButton.isPressed;
#else
        return Input.GetMouseButton(2);
#endif
            }
        }
    }
}