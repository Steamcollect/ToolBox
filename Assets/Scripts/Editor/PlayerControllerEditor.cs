using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    private Color headerColor = new Color(0.2f, 0.4f, 0.7f);

    protected override void OnHeaderGUI()
    {
        // Dessine d'abord le header Unity classique
        base.OnHeaderGUI();

        // Récupère le rect du header
        Rect rect = GUILayoutUtility.GetLastRect();

        // On déplace un peu pour se placer dans la zone des boutons (à droite du titre)
        Rect buttonRect = new Rect(rect.xMax - 40, rect.y + 2, 18, 18);

        // Style bouton icône
        GUIStyle iconStyle = new GUIStyle(GUI.skin.GetStyle("IconButton"));

        // Icône Unity intégrée
        GUIContent icon = EditorGUIUtility.IconContent("d_PlayButton");
        icon.tooltip = "Exécuter une action sur le Player";

        // Dessine le bouton dans le header
        if (GUI.Button(buttonRect, icon, iconStyle))
        {
            PlayerController pc = (PlayerController)target;
            Debug.Log($"Bouton du header cliqué ! Health={pc.health}, Damage={pc.damage}");
        }
    }

}
