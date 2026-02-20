#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonDrawer : Editor
{
    // Store parameter values for each method
    private Dictionary<string, object[]> methodParameters = new Dictionary<string, object[]>();
    private Dictionary<string, bool> methodFoldouts = new Dictionary<string, bool>();

    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        // Get all methods with ButtonAttribute
        MethodInfo[] methods = target.GetType().GetMethods(
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        bool foundAnyButtons = false;

        foreach (MethodInfo method in methods)
        {
            ButtonAttribute buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
            if (buttonAttribute != null)
            {
                if (!foundAnyButtons)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Button Methods", EditorStyles.boldLabel);
                    foundAnyButtons = true;
                }

                DrawMethodButton(method, buttonAttribute);
            }
        }
    }

    private void DrawMethodButton(MethodInfo method, ButtonAttribute buttonAttribute)
    {
        string methodKey = method.Name;
        ParameterInfo[] parameters = method.GetParameters();

        // Initialize parameter storage if needed
        if (!methodParameters.ContainsKey(methodKey))
        {
            methodParameters[methodKey] = new object[parameters.Length];
            InitializeDefaultValues(methodKey, parameters);
        }

        // Draw parameters if method has any
        if (parameters.Length > 0)
        {
            // Create foldout for parameters
            if (!methodFoldouts.ContainsKey(methodKey))
                methodFoldouts[methodKey] = false;

            methodFoldouts[methodKey] = EditorGUILayout.Foldout(
                methodFoldouts[methodKey],
                $"{GetButtonText(method, buttonAttribute)} Parameters",
                true
            );

            if (methodFoldouts[methodKey])
            {
                EditorGUI.indentLevel++;
                DrawParameters(methodKey, parameters);
                EditorGUI.indentLevel--;
            }
        }

        // Draw the button
        string buttonText = GetButtonText(method, buttonAttribute);
        GUILayoutOption heightOption = GUILayout.Height(buttonAttribute.ButtonHeight);

        if (GUILayout.Button(buttonText, heightOption))
        {
            InvokeMethod(method, methodKey, parameters);
        }

        EditorGUILayout.Space();
    }

    private void DrawParameters(string methodKey, ParameterInfo[] parameters)
    {
        object[] paramValues = methodParameters[methodKey];

        for (int i = 0; i < parameters.Length; i++)
        {
            ParameterInfo param = parameters[i];
            Type paramType = param.ParameterType;
            string paramName = ObjectNames.NicifyVariableName(param.Name);

            // Draw appropriate field based on parameter type
            if (paramType == typeof(int))
            {
                paramValues[i] = EditorGUILayout.IntField(paramName, (int)(paramValues[i] ?? 0));
            }
            else if (paramType == typeof(float))
            {
                paramValues[i] = EditorGUILayout.FloatField(paramName, (float)(paramValues[i] ?? 0f));
            }
            else if (paramType == typeof(double))
            {
                paramValues[i] = EditorGUILayout.DoubleField(paramName, (double)(paramValues[i] ?? 0.0));
            }
            else if (paramType == typeof(string))
            {
                paramValues[i] = EditorGUILayout.TextField(paramName, (string)(paramValues[i] ?? ""));
            }
            else if (paramType == typeof(bool))
            {
                paramValues[i] = EditorGUILayout.Toggle(paramName, (bool)(paramValues[i] ?? false));
            }
            else if (paramType == typeof(Vector2))
            {
                paramValues[i] = EditorGUILayout.Vector2Field(paramName, (Vector2)(paramValues[i] ?? Vector2.zero));
            }
            else if (paramType == typeof(Vector3))
            {
                paramValues[i] = EditorGUILayout.Vector3Field(paramName, (Vector3)(paramValues[i] ?? Vector3.zero));
            }
            else if (paramType == typeof(Vector4))
            {
                paramValues[i] = EditorGUILayout.Vector4Field(paramName, (Vector4)(paramValues[i] ?? Vector4.zero));
            }
            else if (paramType == typeof(Color))
            {
                paramValues[i] = EditorGUILayout.ColorField(paramName, (Color)(paramValues[i] ?? Color.white));
            }
            else if (paramType.IsEnum)
            {
                if (paramValues[i] == null)
                    paramValues[i] = Enum.GetValues(paramType).GetValue(0);
                paramValues[i] = EditorGUILayout.EnumPopup(paramName, (Enum)paramValues[i]);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(paramType))
            {
                paramValues[i] = EditorGUILayout.ObjectField(paramName, (UnityEngine.Object)paramValues[i], paramType, true);
            }
            else
            {
                // For unsupported types, show a label
                EditorGUILayout.LabelField(paramName, $"Unsupported type: {paramType.Name}");
            }
        }
    }

    private void InitializeDefaultValues(string methodKey, ParameterInfo[] parameters)
    {
        object[] paramValues = methodParameters[methodKey];

        for (int i = 0; i < parameters.Length; i++)
        {
            Type paramType = parameters[i].ParameterType;

            // Set appropriate default values
            if (paramType == typeof(int))
                paramValues[i] = 0;
            else if (paramType == typeof(float))
                paramValues[i] = 0f;
            else if (paramType == typeof(double))
                paramValues[i] = 0.0;
            else if (paramType == typeof(string))
                paramValues[i] = "";
            else if (paramType == typeof(bool))
                paramValues[i] = false;
            else if (paramType == typeof(Vector2))
                paramValues[i] = Vector2.zero;
            else if (paramType == typeof(Vector3))
                paramValues[i] = Vector3.zero;
            else if (paramType == typeof(Vector4))
                paramValues[i] = Vector4.zero;
            else if (paramType == typeof(Color))
                paramValues[i] = Color.white;
            else if (paramType.IsEnum)
                paramValues[i] = Enum.GetValues(paramType).GetValue(0);
            else if (typeof(UnityEngine.Object).IsAssignableFrom(paramType))
                paramValues[i] = null;
            else
                paramValues[i] = null;
        }
    }

    private string GetButtonText(MethodInfo method, ButtonAttribute buttonAttribute)
    {
        return !string.IsNullOrEmpty(buttonAttribute.ButtonText)
            ? buttonAttribute.ButtonText
            : ObjectNames.NicifyVariableName(method.Name);
    }

    private void InvokeMethod(MethodInfo method, string methodKey, ParameterInfo[] parameters)
    {
        try
        {
            object[] paramValues = methodParameters[methodKey];

            // Invoke method on all selected targets
            foreach (var t in targets)
            {
                if (method.IsStatic)
                {
                    method.Invoke(null, paramValues);
                }
                else
                {
                    method.Invoke(t, paramValues);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error invoking method {method.Name}: {e.Message}");
        }
    }
}
#endif