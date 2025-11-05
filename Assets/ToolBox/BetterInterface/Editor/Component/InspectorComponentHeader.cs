using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterInterface
{
    public class InspectorComponentHeader
    {
        private static Dictionary<Type, MethodInfo> methodDict = new Dictionary<Type, MethodInfo>();

        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.update += InitHeader;
        }

        private static void InitHeader()
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

            FieldInfo fieldInfo = typeof(EditorGUIUtility).GetField("s_EditorHeaderItemsMethods", flags);
            IList value = (IList)fieldInfo.GetValue(null);
            if (value == null) return;

            value.Clear();

            Type delegateType = value.GetType().GetGenericArguments()[0];

            Func<Rect, UnityEngine.Object[], bool> func = DrawHeaderItem;
            MethodInfo method = typeof(InspectorComponentHeader)
                .GetMethod("DrawButtons", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (Type type in TypeCache.GetTypesDerivedFrom<Component>())
            {
                if (!methodDict.ContainsKey(type))
                    methodDict.Add(type, method);
            }

            value.Add(Delegate.CreateDelegate(delegateType, func.Method));

            EditorApplication.update -= InitHeader;
        }
        private static bool DrawHeaderItem(Rect rect, UnityEngine.Object[] targets)
        {
            UnityEngine.Object target = targets[0];

            Type targetType = target.GetType();

            if (methodDict.ContainsKey(targetType))
            {
                methodDict.TryGetValue(targetType, out MethodInfo method);
                ParameterInfo[] parameters = method.GetParameters();
                List<Type> parametersType = new List<Type>();

                foreach (var parameter in parameters)
                {
                    parametersType.Add(parameter.ParameterType);
                }

                Type rectType = rect.GetType();

                if (parametersType.Count == 2 &&
                    parametersType[0] == typeof(Rect) &&
                    typeof(UnityEngine.Object).IsAssignableFrom(parametersType[1]))
                {
                    method.Invoke(null, new object[] { rect, target });
                }
                else
                {
                    GUIStyle errorStyle = new GUIStyle();
                    errorStyle.normal.textColor = Color.red;
                    errorStyle.alignment = TextAnchor.MiddleCenter;
                    errorStyle.fontStyle = FontStyle.Bold;
                    errorStyle.fontSize = 12;
                    rect.width = 78;
                    rect.x -= 63;

                    string errorToolTip = "Method Parameter Error: Please make sure the parameter is correct";
                    GUI.Label(rect, new GUIContent(" Item Error!", EditorGUIUtility.IconContent("CollabError").image, errorToolTip), errorStyle);
                }
                return false;
            }
            return false;
        }

        static void DrawButtons(Rect rect, UnityEngine.Object target)
        {
            Component comp = target as Component;
            if (comp == null) return;

            rect.x += 2;

            // Icon Button Style
            GUIStyle iconStyle = new GUIStyle(GUI.skin.GetStyle("IconButton"));

            if (GUI.Button(rect, new GUIContent("", EditorGUIUtility.IconContent("d_P4_AddedRemote").image,
                "Open in new Tab"), iconStyle))
            {
                ToolBox.BetterInterface.SingleComponentWindow.Show(comp);
            }
        }
    }
}