﻿using UnityEditor;
using UnityEngine;

public static class ComponentHeaderButton
{
    [MenuItem("CONTEXT/Component/Open in new Tab", false, 1)]
    static void Test(MenuCommand command)
    {
        SingleComponentWindow.Show((Component)command.context);
    }
}