using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Spreadsheet
{
    public class ScriptableObjectSpeadsheetWindow : EditorWindow
    {
        // folder selector
        DefaultAsset folderObject;
        string folderPath = string.Empty;

        // type selector (assign the script asset for the ScriptableObject-derived class)
        MonoScript typeScript;
        Type sampleType;

        List<ScriptableObject> assets = new List<ScriptableObject>();
        List<string> propertyPaths = new List<string>();

        Vector2 scrollPos;

        // track name edits per asset path
        Dictionary<string, string> nameEdits = new Dictionary<string, string>();

        // selection
        string selectedAssetPath = null;

        // create new
        string createName = "";

        [MenuItem("Window/ScriptableObject Spreadsheet")]
        public static void Open()
        {
            GetWindow<ScriptableObjectSpeadsheetWindow>("ScriptableObject Spreadsheet");
        }

        // Context menu: right click a folder in Project and open window with that folder
        [MenuItem("Assets/Open SO Spreadsheet Here", false, 2000)]
        public static void OpenHere()
        {
            var obj = Selection.activeObject;
            if (obj == null) return;
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) return;

            var win = GetWindow<ScriptableObjectSpeadsheetWindow>("SO Spreadsheet");
            win.folderObject = obj as DefaultAsset;
            win.folderPath = path;
            win.AutoDetectTypeAndScan();
        }

        [MenuItem("Assets/Open SO Spreadsheet Here", true)]
        public static bool ValidateOpenHere()
        {
            var obj = Selection.activeObject;
            if (obj == null) return false;
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) return false;
            // ensure it's a folder
            return AssetDatabase.IsValidFolder(path);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            folderObject = (DefaultAsset)EditorGUILayout.ObjectField("Folder (path)", folderObject, typeof(DefaultAsset), false);
            if (folderObject != null)
                folderPath = AssetDatabase.GetAssetPath(folderObject);
            else
                folderPath = string.Empty;

            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                ScanFolderAndType();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            typeScript = (MonoScript)EditorGUILayout.ObjectField("SO Type (assign script)", typeScript, typeof(MonoScript), false);
            if (GUILayout.Button("Scan", GUILayout.Width(80)))
            {
                ScanFolderAndType();
            }
            EditorGUILayout.EndHorizontal();

            // Create new controls (enabled only if type valid)
            if (typeScript != null && typeScript.GetClass() != null && typeof(ScriptableObject).IsAssignableFrom(typeScript.GetClass()))
            {
                EditorGUILayout.BeginHorizontal();
                createName = EditorGUILayout.TextField("Create New", createName);
                if (GUILayout.Button("Create", GUILayout.Width(80)))
                {
                    CreateNewAsset();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (typeScript == null || typeScript.GetClass() == null || !typeof(ScriptableObject).IsAssignableFrom(typeScript.GetClass()))
            {
                EditorGUILayout.HelpBox("Assign a ScriptableObject-derived script (MonoScript) and a folder to scan. The window will find all assets of that type in the folder and display a spreadsheet. You can also right-click a folder in the Project window and choose 'Open SO Spreadsheet Here' to auto-detect the most common SO type.", MessageType.Info);
                return;
            }

            sampleType = typeScript.GetClass();

            EditorGUILayout.Space();

            // Header
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(20)); // selection column
            GUILayout.Label("Name", GUILayout.Width(200));
            foreach (var path in propertyPaths)
            {
                GUILayout.Label(path, GUILayout.Width(150));
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Rows
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            bool needRescan = false;

            foreach (var so in assets)
            {
                if (so == null) continue;

                SerializedObject sObj = new SerializedObject(so);
                sObj.Update();

                EditorGUILayout.BeginHorizontal();

                string assetPath = AssetDatabase.GetAssetPath(so);
                if (!nameEdits.ContainsKey(assetPath))
                    nameEdits[assetPath] = so.name;

                // selection toggle
                bool isSelected = selectedAssetPath == assetPath;
                bool newSel = GUILayout.Toggle(isSelected, "", GUILayout.Width(20));
                if (newSel != isSelected)
                {
                    if (newSel)
                    {
                        selectedAssetPath = assetPath;
                        Selection.activeObject = so;
                    }
                    else
                    {
                        selectedAssetPath = null;
                        Selection.activeObject = null;
                    }
                }

                // First column is a simple editable string for the asset name.
                string currentField = nameEdits[assetPath];
                string newField = EditorGUILayout.DelayedTextField(currentField, GUILayout.Width(200));
                if (newField != currentField)
                {
                    nameEdits[assetPath] = newField;
                    // perform rename if different from actual asset name
                    if (!string.IsNullOrEmpty(newField) && newField != so.name)
                    {
                        string err = AssetDatabase.RenameAsset(assetPath, newField);
                        if (!string.IsNullOrEmpty(err))
                        {
                            EditorUtility.DisplayDialog("Rename failed", err, "OK");
                        }
                        else
                        {
                            needRescan = true;
                            // don't call ScanFolderAndType() here to avoid modifying collections during iteration
                        }
                    }
                }

                bool anyApplied = false;
                foreach (var path in propertyPaths)
                {
                    SerializedProperty sp = sObj.FindProperty(path);
                    if (sp != null)
                    {
                        // Draw property with minimal label space
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(sp, GUIContent.none, GUILayout.Width(150));
                        if (EditorGUI.EndChangeCheck())
                        {
                            anyApplied = true;
                        }
                    }
                    else
                    {
                        GUILayout.Label("-", GUILayout.Width(150));
                    }
                }

                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                {
                    EditorGUIUtility.PingObject(so);
                }

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Delete asset?", $"Delete '{so.name}'? This will remove the asset file.", "Delete", "Cancel"))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                        needRescan = true;
                    }
                }

                sObj.ApplyModifiedProperties();

                if (anyApplied)
                {
                    EditorUtility.SetDirty(so);
                    AssetDatabase.SaveAssets();
                }

                EditorGUILayout.EndHorizontal();

                // draw selection highlight
                if (selectedAssetPath == assetPath)
                {
                    var lastRect = GUILayoutUtility.GetLastRect();
                    EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y, position.width, lastRect.height), new Color(0.24f, 0.48f, 0.90f, 0.12f));
                }
            }

            EditorGUILayout.EndScrollView();

            if (needRescan)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                ScanFolderAndType();
            }
        }

        void CreateNewAsset()
        {
            if (typeScript == null) return;
            var cls = typeScript.GetClass();
            if (cls == null || !typeof(ScriptableObject).IsAssignableFrom(cls)) return;

            string targetFolder = string.IsNullOrEmpty(folderPath) ? "Assets" : folderPath;
            string baseName = string.IsNullOrEmpty(createName) ? "New" + cls.Name : createName;
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(targetFolder + "/" + baseName + ".asset");

            var instance = ScriptableObject.CreateInstance(cls);
            AssetDatabase.CreateAsset(instance, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // select created
            var newObj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(uniquePath);
            if (newObj != null)
            {
                selectedAssetPath = uniquePath;
                Selection.activeObject = newObj;
            }

            ScanFolderAndType();
        }

        void ScanFolderAndType()
        {
            assets.Clear();
            propertyPaths.Clear();
            nameEdits.Clear();

            if (typeScript == null)
                return;

            var cls = typeScript.GetClass();
            if (cls == null || !typeof(ScriptableObject).IsAssignableFrom(cls))
                return;

            sampleType = cls;

            string[] guids;
            if (!string.IsNullOrEmpty(folderPath))
                guids = AssetDatabase.FindAssets($"t:{sampleType.Name}", new[] { folderPath });
            else
                guids = AssetDatabase.FindAssets($"t:{sampleType.Name}");

            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var obj = AssetDatabase.LoadAssetAtPath(path, sampleType) as ScriptableObject;
                if (obj != null)
                    assets.Add(obj);
            }

            // collect property paths from first asset if available
            if (assets.Count > 0)
            {
                var sObj = new SerializedObject(assets[0]);
                var prop = sObj.GetIterator();
                if (prop.NextVisible(true))
                {
                    do
                    {
                        if (prop.name == "m_Script")
                            continue;

                        propertyPaths.Add(prop.propertyPath);
                    }
                    while (prop.NextVisible(false));
                }
            }
        }

        void AutoDetectTypeAndScan()
        {
            // find all ScriptableObject assets in the folder recursively
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });
            if (guids == null || guids.Length == 0)
            {
                // nothing found
                ScanFolderAndType();
                return;
            }

            // count occurrences by exact type
            var counts = new Dictionary<Type, List<ScriptableObject>>();
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (obj == null) continue;
                Type t = obj.GetType();
                if (!counts.ContainsKey(t)) counts[t] = new List<ScriptableObject>();
                counts[t].Add(obj);
            }

            if (counts.Count == 0)
            {
                ScanFolderAndType();
                return;
            }

            // pick most common type
            var best = counts.OrderByDescending(kv => kv.Value.Count).First();
            var bestType = best.Key;
            var sample = best.Value.FirstOrDefault();

            if (sample != null)
            {
                // try to find MonoScript for this sample
                var ms = MonoScript.FromScriptableObject(sample);
                if (ms != null)
                {
                    typeScript = ms;
                }
                else
                {
                    typeScript = null;
                }
            }

            // finally scan according to detected type
            ScanFolderAndType();
        }
    }
}