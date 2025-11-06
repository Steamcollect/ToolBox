using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterInterface
{
    [InitializeOnLoad]
    public static class ComponentDrawer
    {
        static ComponentDrawer()
        {
            Editor.finishedDefaultHeaderGUI += OnGUI;
        }

        private static void OnGUI(Editor editor)
        {
            if (editor == null || editor.target == null)
                return;

            // On ne veut que les vrais composants (Transform, MeshRenderer, scripts, etc.)
            if (!(editor.target is Component component))
                return;

            // On récupère la zone du header en cours
            // Chaque header occupe environ 18-22 px de hauteur selon l’UI scale
            Rect headerRect = GUILayoutUtility.GetLastRect();

            // Vérifie qu’on est dans une zone valide (évite les appels hors repaint)
            if (Event.current.type != EventType.Repaint)
                return;

            // Calculer la zone du carré (par ex : 6 px de marge à gauche)
            Rect squareRect = new Rect(headerRect.x + 4, headerRect.y + 2, 14, 14);

            // Dessine le carré rouge
            Color prev = GUI.color;
            GUI.color = Color.red;
            GUI.Box(squareRect, GUIContent.none);
            GUI.color = prev;

            // Optionnel : affiche le nom du composant dans le carré
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 8,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            GUI.Label(squareRect, "C", style);
        }
    }
}