using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.Dev
{
    [CustomPropertyDrawer(typeof(object), true)]
    public class SerializeReferenceDrawer_PropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, Dictionary<string, Type>> s_TypeCache = new();
        private const float k_HeaderPadding = 2f;
        private const float k_ContentPadding = 4f;
        private const float k_FoldoutWidth = 12f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUI.GetPropertyHeight(property, label, true);

            var baseType = GetBaseType(property);
            if (baseType == null || (!baseType.IsAbstract && !baseType.IsInterface))
                return EditorGUI.GetPropertyHeight(property, label, true);

            float height = EditorGUIUtility.singleLineHeight + k_HeaderPadding * 2;

            if (property.isExpanded && property.managedReferenceValue != null)
            {
                var childHeight = 0f;
                var copy = property.Copy();
                var end = copy.GetEndProperty();
                copy.NextVisible(true);
                
                while (!SerializedProperty.EqualContents(copy, end))
                {
                    childHeight += EditorGUI.GetPropertyHeight(copy, true) + EditorGUIUtility.standardVerticalSpacing;
                    if (!copy.NextVisible(false))
                        break;
                }
                
                height += childHeight + k_ContentPadding * 2;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            
            var baseType = GetBaseType(property);
            if (baseType == null || (!baseType.IsAbstract && !baseType.IsInterface))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            if (!s_TypeCache.ContainsKey(baseType))
                BuildTypeCache(baseType);

            EditorGUI.BeginProperty(position, label, property);

            var typeName = property.managedReferenceFullTypename;
            var displayTypeName = string.IsNullOrEmpty(typeName) ? "None" : GetShortTypeName(typeName);
            
            // Draw background box
            var boxRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + k_HeaderPadding * 2);
            DrawStyledBackground(boxRect);

            // Header row layout
            var headerRect = new Rect(position.x + k_HeaderPadding, position.y + k_HeaderPadding, position.width - k_HeaderPadding * 2, EditorGUIUtility.singleLineHeight);
            
            // Foldout area (only if there's content to expand)
            var hasFoldout = property.managedReferenceValue != null;
            var foldoutRect = new Rect(headerRect.x, headerRect.y, k_FoldoutWidth, headerRect.height);
            
            if (hasFoldout)
            {
                EditorGUI.BeginChangeCheck();
                var newExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none, true);
                if (EditorGUI.EndChangeCheck())
                {
                    property.isExpanded = newExpanded;
                }
            }

            // Label area
            var labelStartX = headerRect.x + (hasFoldout ? k_FoldoutWidth + 2 : 0);
            var labelRect = new Rect(labelStartX, headerRect.y, EditorGUIUtility.labelWidth - labelStartX + headerRect.x, headerRect.height);
            
            var labelStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
            EditorGUI.LabelField(labelRect, label, labelStyle);

            // Type display and dropdown button
            var dropdownButtonRect = new Rect(labelRect.xMax + 2, headerRect.y, headerRect.xMax - labelRect.xMax - 2, headerRect.height);
            
            var typeDisplayStyle = new GUIStyle(EditorStyles.popup)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Normal,
                fontSize = 11
            };

            var typeColor = string.IsNullOrEmpty(typeName) ? new Color(0.5f, 0.5f, 0.5f, 1f) : new Color(0.3f, 0.6f, 0.9f, 1f);
            var originalColor = GUI.color;
            GUI.color = typeColor;

            if (EditorGUI.DropdownButton(dropdownButtonRect, new GUIContent($"  {displayTypeName}"), FocusType.Keyboard, typeDisplayStyle))
            {
                ShowTypeSelectionMenu(property, baseType, typeName);
            }

            GUI.color = originalColor;

            // Draw children if expanded
            if (property.isExpanded && property.managedReferenceValue != null)
            {
                var contentRect = new Rect(
                    position.x + k_ContentPadding,
                    boxRect.yMax + k_ContentPadding,
                    position.width - k_ContentPadding * 2,
                    position.height - boxRect.height - k_ContentPadding * 2
                );

                DrawChildProperties(property, contentRect);
            }

            EditorGUI.EndProperty();
        }

        private void DrawStyledBackground(Rect rect)
        {
            var bgColor = EditorGUIUtility.isProSkin 
                ? new Color(0.25f, 0.25f, 0.25f, 1f) 
                : new Color(0.85f, 0.85f, 0.85f, 1f);

            if (rect.Contains(Event.current.mousePosition))
            {
                bgColor = EditorGUIUtility.isProSkin 
                    ? new Color(0.3f, 0.3f, 0.3f, 1f) 
                    : new Color(0.8f, 0.8f, 0.8f, 1f);
            }

            EditorGUI.DrawRect(rect, bgColor);
            
            // Draw subtle border
            var borderColor = EditorGUIUtility.isProSkin 
                ? new Color(0.15f, 0.15f, 0.15f, 1f) 
                : new Color(0.6f, 0.6f, 0.6f, 1f);
            
            // Top border
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), borderColor);
            // Bottom border
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), borderColor);
            // Left border
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), borderColor);
            // Right border
            EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y, 1, rect.height), borderColor);
        }

        private void DrawChildProperties(SerializedProperty property, Rect contentRect)
        {
            var yOffset = 0f;
            var copy = property.Copy();
            var end = copy.GetEndProperty();
            var indent = EditorGUI.indentLevel;
            
            EditorGUI.indentLevel++;
            
            if (copy.NextVisible(true))
            {
                while (!SerializedProperty.EqualContents(copy, end))
                {
                    var propHeight = EditorGUI.GetPropertyHeight(copy, true);
                    var propRect = new Rect(contentRect.x, contentRect.y + yOffset, contentRect.width, propHeight);
                    
                    EditorGUI.PropertyField(propRect, copy, true);
                    
                    yOffset += propHeight + EditorGUIUtility.standardVerticalSpacing;
                    
                    if (!copy.NextVisible(false))
                        break;
                }
            }
            
            EditorGUI.indentLevel = indent;
        }

        private void ShowTypeSelectionMenu(SerializedProperty property, Type baseType, string currentTypeName)
        {
            var menu = new GenericMenu();
            var types = s_TypeCache[baseType];

            menu.AddItem(new GUIContent("None"), string.IsNullOrEmpty(currentTypeName), () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddSeparator("");

            if (types == null || types.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Types Found"));
            }
            else
            {
                foreach (var type in types.OrderBy(t => t.Key))
                {
                    var name = type.Key;
                    var typeValue = type.Value;
                    var isSelected = typeValue.FullName == GetFullTypeNameFromReference(currentTypeName);
                    
                    menu.AddItem(new GUIContent(name), isSelected, () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(typeValue);
                        property.isExpanded = true;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            
            menu.ShowAsContext();
        }

        private string GetFullTypeNameFromReference(string managedReferenceFullTypename)
        {
            if (string.IsNullOrEmpty(managedReferenceFullTypename))
                return null;
            
            var parts = managedReferenceFullTypename.Split(' ');
            return parts.Length > 1 ? parts[1] : managedReferenceFullTypename;
        }

        private Type GetBaseType(SerializedProperty property)
        {
            var fullTypeName = property.managedReferenceFieldTypename;
            if (string.IsNullOrEmpty(fullTypeName)) return null;

            var parts = fullTypeName.Split(' ');
            if (parts.Length != 2) return null;

            var assemblyName = parts[0];
            var typeName = parts[1];

            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            return assembly?.GetType(typeName);
        }

        private void BuildTypeCache(Type baseType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm =>
                {
                    try { return asm.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => !t.IsAbstract && !t.IsInterface && baseType.IsAssignableFrom(t))
                .ToDictionary(t => ObjectNames.NicifyVariableName(t.Name), t => t);

            s_TypeCache[baseType] = types;
        }

        private string GetShortTypeName(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName)) return null;
            var parts = fullTypeName.Split(' ');
            return parts.Length > 1 ? parts[1].Split('.').Last() : fullTypeName;
        }
        
    }

}



