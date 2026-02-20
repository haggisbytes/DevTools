#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UnityEngine.Object), true)]
public class ConditionalFieldEditor : Editor
{
    private Dictionary<string, bool> conditionalVisibility = new Dictionary<string, bool>();

    public override void OnInspectorGUI()
    {
        // Update visibility states
        UpdateConditionalVisibility();
        // Draw properties with conditional visibility
        serializedObject.Update();
        DrawPropertiesWithConditionalVisibility();
        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateConditionalVisibility()
    {
        conditionalVisibility.Clear();
        Type targetType = target.GetType();
        FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            ShowIfAttribute showIf = field.GetCustomAttribute<ShowIfAttribute>();
            HideIfAttribute hideIf = field.GetCustomAttribute<HideIfAttribute>();

            if (showIf != null)
            {
                bool conditionMet = EvaluateCondition(showIf.ConditionFieldName, showIf.ExpectedValue);
                conditionalVisibility[field.Name] = conditionMet;
            }
            else if (hideIf != null)
            {
                bool conditionMet = EvaluateCondition(hideIf.ConditionFieldName, hideIf.ExpectedValue);
                conditionalVisibility[field.Name] = !conditionMet;
            }
        }
    }

    private bool EvaluateCondition(string conditionFieldName, object expectedValue)
    {
        SerializedProperty conditionProperty = serializedObject.FindProperty(conditionFieldName);

        if (conditionProperty != null)
        {
            switch (conditionProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    if (expectedValue is bool)
                        return conditionProperty.boolValue == (bool)expectedValue;
                    break;

                case SerializedPropertyType.Enum:
                    if (expectedValue is Enum)
                    {
                        return conditionProperty.enumValueIndex == Convert.ToInt32(expectedValue);
                    }
                    else if (expectedValue is int)
                    {
                        return conditionProperty.enumValueIndex == (int)expectedValue;
                    }
                    else if (expectedValue is string)
                    {
                        return conditionProperty.enumNames[conditionProperty.enumValueIndex] == (string)expectedValue;
                    }
                    break;

                case SerializedPropertyType.Integer:
                    if (expectedValue != null)
                        return conditionProperty.intValue == Convert.ToInt32(expectedValue);
                    break;

                case SerializedPropertyType.Float:
                    if (expectedValue != null)
                        return Mathf.Approximately(conditionProperty.floatValue, Convert.ToSingle(expectedValue));
                    break;

                case SerializedPropertyType.String:
                    if (expectedValue != null)
                        return conditionProperty.stringValue == expectedValue.ToString();
                    break;
            }
        }

        // Fallback to reflection for non-serialized properties
        Type targetType = target.GetType();
        FieldInfo field = targetType.GetField(conditionFieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            object fieldValue = field.GetValue(target);

            if (fieldValue != null && expectedValue != null)
            {
                // Handle enums
                if (field.FieldType.IsEnum && expectedValue is Enum)
                {
                    return Convert.ToInt32(fieldValue) == Convert.ToInt32(expectedValue);
                }
                // Handle other types
                return fieldValue.Equals(expectedValue);
            }
        }

        PropertyInfo prop = targetType.GetProperty(conditionFieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (prop != null)
        {
            object propValue = prop.GetValue(target);

            if (propValue != null && expectedValue != null)
            {
                // Handle enums
                if (prop.PropertyType.IsEnum && expectedValue is Enum)
                {
                    return Convert.ToInt32(propValue) == Convert.ToInt32(expectedValue);
                }
                // Handle other types
                return propValue.Equals(expectedValue);
            }
        }

        return true; // Show by default if condition not found
    }

    private void DrawPropertiesWithConditionalVisibility()
    {
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            // Skip script field
            if (iterator.propertyPath == "m_Script")
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(iterator, true);
                GUI.enabled = true;
                continue;
            }

            // Check if this property should be visible
            if (conditionalVisibility.ContainsKey(iterator.name))
            {
                if (conditionalVisibility[iterator.name])
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
            else
            {
                // No conditional attribute, draw normally
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }
}
#endif