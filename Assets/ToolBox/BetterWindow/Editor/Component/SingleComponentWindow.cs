using UnityEditor;
using UnityEngine;

public class SingleComponentWindow : EditorWindow
{
    private Component targetComponent;
    private Editor componentEditor;

    public static void Show(Component component)
    {
        if (component == null)
        {
            Debug.LogWarning("Aucun composant fourni.");
            return;
        }

        var window = CreateInstance<SingleComponentWindow>();
        window.targetComponent = component;
        window.titleContent = new GUIContent($"{component.GetType().Name}");
        window.CreateEditor();

        // Calcul de la hauteur en fonction du contenu
        Vector2 contentSize = window.CalculateContentSize();

        // Taille initiale : largeur raisonnable + hauteur adaptée
        float initialWidth = 400f;
        float height = contentSize.y;

        Rect rect = new Rect(
            (Screen.currentResolution.width - initialWidth) / 2f,
            (Screen.currentResolution.height - height) / 2f,
            initialWidth,
            height
        );

        window.position = rect;

        window.minSize = new Vector2(300, height);
        window.maxSize = new Vector2(800f, height);

        window.ShowUtility();
    }

    private void CreateEditor()
    {
        if (targetComponent != null)
            componentEditor = Editor.CreateEditor(targetComponent);
    }

    /// <summary>
    /// Calcule la hauteur en fonction du nombre de propriétés visibles dans l’inspecteur.
    /// </summary>
    private Vector2 CalculateContentSize()
    {
        float width = 400f;
        float height = 100f;

        if (targetComponent != null)
        {
            var so = new SerializedObject(targetComponent);
            var prop = so.GetIterator();
            int visibleCount = 0;
            while (prop.NextVisible(true))
                visibleCount++;

            // Ajustement de la hauteur selon le nombre de lignes visibles
            float lineHeight = EditorGUIUtility.singleLineHeight + 2f;
            height = Mathf.Clamp(visibleCount * lineHeight + 60f, 120f, 800f);
        }

        return new Vector2(width, height);
    }

    private void OnGUI()
    {
        if (targetComponent == null)
        {
            EditorGUILayout.HelpBox("Aucun composant à afficher.", MessageType.Info);
            return;
        }

        if (componentEditor == null)
            CreateEditor();

        EditorGUI.BeginChangeCheck();
        componentEditor.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(targetComponent);
    }

    private void OnDisable()
    {
        if (componentEditor != null)
            DestroyImmediate(componentEditor);
    }
}