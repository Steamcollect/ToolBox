using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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
        Type delegateType = value.GetType().GetGenericArguments()[0];

        Func<Rect, UnityEngine.Object[], bool> func = DrawHeaderItem;

        // Récupérer la méthode DrawButtons
        MethodInfo method = typeof(InspectorComponentHeader)
            .GetMethod("DrawButtons", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        // Enregistrer DrawButtons pour tous les types dérivés de Component
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

            if (parametersType.Count == 1 && parametersType.Contains(rectType))
            {
                method.Invoke(null, new object[] { rect });
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

    static void DrawButtons(Rect rect)
    {
        // Icon Button Style
        GUIStyle iconStyle = new GUIStyle(GUI.skin.GetStyle("IconButton"));

        //Using Unity's built-in Icon requires adjusting the location of each Icon
        iconStyle.contentOffset = new Vector2(2.5f, 1.5f);
        //Draw Button
        if (GUI.Button(rect, new GUIContent("", EditorGUIUtility.IconContent("Clipboard").image, "Displays a list of languages"), iconStyle))
        {
            Debug.Log("Test 1");
        }

        //Adjust the Rect to avoid overlap between the two buttons
        rect.x -= 19;

        //Using Unity's built-in Icon requires adjusting the location of each Icon
        iconStyle.contentOffset = new Vector2(2f, 2f);
        //Draw Button
        if (GUI.Button(rect, new GUIContent("", EditorGUIUtility.IconContent("RectTransformRaw").image, "Switch a language at random"), iconStyle))
        {
            Debug.Log("Test 1");
        }
    }
}