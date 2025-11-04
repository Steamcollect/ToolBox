using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string folderName;
    public int health;
    public int damage;

//#if UNITY_EDITOR
//    [ComponentHeaderItem]
//    private static void DrawHeaderButton(Rect rect)
//    {
//        // Style du bouton (comme dans tes exemples)
//        GUIStyle iconStyle = new GUIStyle(GUI.skin.GetStyle("IconButton"));

//        // Icône Unity intégrée (tu peux en choisir une autre)
//        var content = UnityEditor.EditorGUIUtility.IconContent("d_PlayButton");
//        content.tooltip = "Bouton du header PlayerController";

//        // Dessine le bouton dans le rect fourni par Unity
//        if (GUI.Button(rect, content, iconStyle))
//        {
//            Debug.Log("Bouton de header PlayerController cliqué !");
//            // Ici tu fais ce que tu veux : reset, ouvrir une fenêtre, etc.
//        }
//    }
//#endif
}
