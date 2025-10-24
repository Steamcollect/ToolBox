using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolBox.BetterWindow
{
    [InitializeOnLoad]
    public class CustomGameObjectHierarchyPopUp : PopupWindowContent
    {
        private Vector2 scroll;

        string[] utilities = new string[] {             
            "GameObject Icon",
            "Folder Icon",
            "cs Script Icon",
            "ScriptableObject Icon",
        };
        string[] components = new string[] {
            "d_Avatar Icon",
            "NavMeshAgent Icon",
            "Camera Icon",
            "ReflectionProbe Icon",
            "d_LightingDataAsset Icon",  
        };
        string[] physics = new string[] {
            "Rigidbody Icon",
            "BoxCollider Icon",
            "SphereCollider Icon",
            "Terrain Icon",
            "PhysicsMaterial Icon",
        };
        string[] yellow = new string[] {
            "Light Icon",
            "DirectionalLight Icon",
            "LightmapParameters Icon",
            "AudioSource Icon",
            "AudioClip Icon",
        };
        string[] grey = new string[]
        {
            "Canvas Icon",
            "Button Icon",
            "Image Icon",
            "Text Icon",
            "Toggle Icon",
        };

        Stack<System.Action> undoStack = new();

        int objId;

        public CustomGameObjectHierarchyPopUp(Object obj)
        {
            objId = obj.GetInstanceID();
        }

        public override void OnGUI(Rect rect)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            const int iconsPerRow = 6;

            GUILayout.Space(10);

            DrawIcons(utilities, iconsPerRow);

            GUILayout.Space(3);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2) });
            GUILayout.Space(3);

            DrawIcons(components, iconsPerRow);

            GUILayout.Space(3);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2) });
            GUILayout.Space(3); 
            
            DrawIcons(physics, iconsPerRow);

            GUILayout.Space(3);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2) });
            GUILayout.Space(3);

            DrawIcons(yellow, iconsPerRow);

            GUILayout.Space(3);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2) });
            GUILayout.Space(3);

            DrawIcons(grey, iconsPerRow);

            GUILayout.Space(10);
            GUILayout.EndScrollView();
        }

        void DrawIcons(string[] icons, int iconsPerRow)
        {
            int count = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            for (int i = 0; i < icons.Length; i++)
            {
                GUIContent iconContent = EditorGUIUtility.IconContent(icons[i]);
                if (iconContent == null || iconContent.image == null)
                    continue;

                if (GUILayout.Button(iconContent, GUIStyle.none, GUILayout.Width(18), GUILayout.Height(18)))
                {
                    Texture2D previousIcon = GameObjectHierarchyEditor.GetData(objId).icon;
                    Texture2D newIcon = (Texture2D)iconContent.image;

                    // Push dans la pile d'Undo
                    undoStack.Push(() => {
                        GameObjectHierarchyEditor.SetData(objId, previousIcon);
                    });

                    // Applique la nouvelle icône
                    GameObjectHierarchyEditor.SetData(objId, newIcon);
                }

                if (i < icons.Length - 1)
                {
                    GUILayout.Space(4);
                }

                count++;
                if (count % iconsPerRow == 0)
                {
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                }
            }

            GUILayout.EndHorizontal();

            Event e = Event.current;
            CheckUndo(e);
        }

        void CheckUndo(Event e)
        {
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
            {
                if (undoStack.Count > 0)
                {
                    undoStack.Pop().Invoke();
                    e.Use(); // Empêche la propagation
                }
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(130, 174);
        }
    }
}