using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterInterface
{
    public static class ComponentMenuItemsButtons
    {
        [MenuItem("CONTEXT/Component/Open in new Tab", false, 1000)]
        static void Test(MenuCommand command)
        {
            SingleComponentWindow.Show((Component)command.context);
        }
    }
}