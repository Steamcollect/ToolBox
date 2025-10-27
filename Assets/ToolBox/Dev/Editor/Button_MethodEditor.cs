using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace ToolBox.Dev
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class Button_MethodEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dessine d’abord l’inspecteur normal
            base.OnInspectorGUI();

            // Récupère toutes les méthodes de l’objet
            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttr == null)
                    continue;

                // Crée le bouton
                if (GUILayout.Button(ObjectNames.NicifyVariableName(method.Name)))
                {
                    object[] parameters = ResolveParameters(buttonAttr.Parameters, target);
                    method.Invoke(target, parameters);
                }
            }
        }

        private object[] ResolveParameters(object[] rawParams, object target)
        {
            if (rawParams == null)
                return null;

            object[] resolved = new object[rawParams.Length];

            for (int i = 0; i < rawParams.Length; i++)
            {
                object param = rawParams[i];
                if (param is string s)
                {
                    // Si c’est le nom d’un champ, on tente de le récupérer
                    var field = target.GetType().GetField(s,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        resolved[i] = field.GetValue(target);
                        continue;
                    }
                }

                // Sinon on garde la valeur brute
                resolved[i] = param;
            }

            return resolved;
        }
    }
}