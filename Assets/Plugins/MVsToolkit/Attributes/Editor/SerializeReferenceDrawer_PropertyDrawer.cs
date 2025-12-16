using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomPropertyDrawer(typeof(SerializeReferenceDrawerAttribute), true)]
    public class SerializeReferenceDrawer_PropertyDrawer : PropertyDrawer
    {
        private const string k_ShortTypeNameDefault = "None";
        private static readonly Dictionary<Type, Dictionary<string, Type>> s_TypeCache = new();
        private static readonly Dictionary<string, Type> s_BaseTypeCache = new(); // Cache des types de base

        private static GUIStyle s_LabelStyle;
        private static GUIStyle s_TypeDisplayStyle;

        private static GUIStyle LabelStyle => s_LabelStyle ??= new GUIStyle(EditorStyles.label);
        private static GUIStyle TypeDisplayStyle => s_TypeDisplayStyle ??= new GUIStyle(EditorStyles.objectField);

        [InitializeOnLoadMethod]
        private static void ClearCacheOnReload()
        {
            s_TypeCache.Clear();
            s_BaseTypeCache.Clear();
            s_LabelStyle = null;
            s_TypeDisplayStyle = null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUI.GetPropertyHeight(property, label, true);

            Type baseType = GetBaseTypeCached(property);
            if (baseType == null || (!baseType.IsAbstract && !baseType.IsInterface))
                return EditorGUI.GetPropertyHeight(property, label, true);

            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded && property.managedReferenceValue != null)
                height += EditorGUIUtility.standardVerticalSpacing + GetChildrenHeight(property);

            return height;
        }

        private float GetChildrenHeight(SerializedProperty property)
        {
            float height = 0f;
            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            if (!iterator.NextVisible(true)) return height;

            do
            {
                if (SerializedProperty.EqualContents(iterator, end)) break;
                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            } while (iterator.NextVisible(false));

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            Type baseType = GetBaseTypeCached(property);
            if (baseType == null || (!baseType.IsAbstract && !baseType.IsInterface))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EnsureTypeCacheBuilt(baseType);

            EditorGUI.BeginProperty(position, label, property);
            DrawHeader(position, property, label, baseType);
            EditorGUI.EndProperty();
        }

        private void DrawHeader(Rect position, SerializedProperty property, GUIContent label, Type baseType)
        {
            string typeName = property.managedReferenceFullTypename;
            string displayTypeName =
                string.IsNullOrEmpty(typeName) ? k_ShortTypeNameDefault : GetShortTypeName(typeName);

            string labelDisplayText = $"{displayTypeName} ({GetBaseTypeCached(property).Name})";

            Rect headerRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            bool hasFoldout = property.managedReferenceValue != null && property.hasVisibleChildren;

            if (hasFoldout)
            {
                Rect foldoutRect = new(headerRect.x, headerRect.y, EditorGUIUtility.labelWidth, headerRect.height);
                property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
            }
            else
            {
                Rect labelRect = new(headerRect.x, headerRect.y, EditorGUIUtility.labelWidth, headerRect.height);
                EditorGUI.LabelField(labelRect, label, LabelStyle);
            }
            
            Rect dropdownRect = new(headerRect.x + EditorGUIUtility.labelWidth + 2, headerRect.y,
                headerRect.width - EditorGUIUtility.labelWidth - 2, headerRect.height);

            DrawTypeDropdown(dropdownRect, labelDisplayText, property, baseType, typeName);

            if (!property.isExpanded || !hasFoldout) return;
            Rect contentRect = new(position.x, headerRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                position.width, position.height - headerRect.height - EditorGUIUtility.standardVerticalSpacing);
            DrawChildProperties(property, contentRect);
        }

        private Type GetBaseTypeCached(SerializedProperty property)
        {
            string fullTypeName = property.managedReferenceFieldTypename;
            if (string.IsNullOrEmpty(fullTypeName)) return null;

            if (s_BaseTypeCache.TryGetValue(fullTypeName, out Type cached))
                return cached;

            string[] parts = fullTypeName.Split(' ');
            if (parts.Length != 2) return null;

            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == parts[0]);
            Type type = assembly?.GetType(parts[1]);

            s_BaseTypeCache[fullTypeName] = type;
            return type;
        }

        private void EnsureTypeCacheBuilt(Type baseType)
        {
            if (s_TypeCache.ContainsKey(baseType)) return;

            s_TypeCache[baseType] = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm =>
                {
                    try
                    {
                        return asm.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(t => !t.IsAbstract && !t.IsInterface && baseType.IsAssignableFrom(t))
                .ToDictionary(t => ObjectNames.NicifyVariableName(t.Name), t => t);
        }

        private static string GetShortTypeName(string fullTypename)
        {
            if (string.IsNullOrEmpty(fullTypename)) return k_ShortTypeNameDefault;
            int lastDot = fullTypename.LastIndexOf('.');
            return lastDot >= 0 ? fullTypename.Substring(lastDot + 1) : fullTypename;
        }

        private void DrawTypeDropdown(Rect rect, string displayText, SerializedProperty property, Type baseType,
            string currentTypeName)
        {
            // Draw the background with objectField style
            if (Event.current.type == EventType.Repaint)
                TypeDisplayStyle.Draw(rect, GUIContent.none, false, false, false, false);

            // Calculate icon rect for the popup arrow (right side)
            float arrowWidth = 16f;
            Rect arrowRect = new(rect.xMax - arrowWidth - 2, rect.y, arrowWidth, rect.height);

            // Draw the text content (with some padding)
            Rect textRect = new(rect.x + 4, rect.y, rect.width - arrowWidth - 6, rect.height);
            EditorGUI.LabelField(textRect, displayText, EditorStyles.label);

            // Draw the popup arrow icon
            if (Event.current.type == EventType.Repaint)
            {
                Rect iconRect = new(arrowRect.x, arrowRect.y + (arrowRect.height - 12) / 2, 12, 12);
                GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("icon dropdown").image);
            }

            // Handle click
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
                ShowTypeSelectionMenu(property, baseType, currentTypeName);
        }

        private void DrawChildProperties(SerializedProperty property, Rect contentRect)
        {
            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            if (!iterator.NextVisible(true)) return;

            float yOffset = contentRect.y;
            EditorGUI.indentLevel++;
            do
            {
                if (SerializedProperty.EqualContents(iterator, end)) break;

                float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                Rect propRect = new(contentRect.x, yOffset, contentRect.width, propHeight);
                EditorGUI.PropertyField(propRect, iterator, true);
                yOffset += propHeight + EditorGUIUtility.standardVerticalSpacing;
            } while (iterator.NextVisible(false));

            EditorGUI.indentLevel--;
        }

        private void ShowTypeSelectionMenu(SerializedProperty property, Type baseType, string currentTypeName)
        {
            GenericMenu menu = new();

            menu.AddItem(new GUIContent(k_ShortTypeNameDefault), string.IsNullOrEmpty(currentTypeName), () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            if (s_TypeCache.TryGetValue(baseType, out Dictionary<string, Type> types))
                foreach (KeyValuePair<string, Type> kvp in types.OrderBy(k => k.Key))
                {
                    Type type = kvp.Value;
                    bool isSelected = currentTypeName != null && currentTypeName.EndsWith(type.FullName ?? type.Name);
                    menu.AddItem(new GUIContent(kvp.Key), isSelected, () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }

            menu.ShowAsContext();
        }
    }
}