#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;

        // Get the target object
        object target = property.serializedObject.targetObject;

        // Get the method info
        MethodInfo method = target.GetType().GetMethod(property.name,
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null)
        {
            GUI.Label(position, $"Method '{property.name}' not found!");
            return;
        }

        // Check if method has parameters
        ParameterInfo[] parameters = method.GetParameters();
        bool hasParameters = parameters.Length > 0;

        // Determine button text
        string buttonText = !string.IsNullOrEmpty(buttonAttribute.ButtonText)
            ? buttonAttribute.ButtonText
            : ObjectNames.NicifyVariableName(property.name);

        // Add warning if method has parameters
        if (hasParameters)
        {
            buttonText += " (Has Parameters)";
        }

        // Disable button if in play mode and method requires it, or if has parameters
        bool wasEnabled = GUI.enabled;
        if (hasParameters)
        {
            GUI.enabled = false;
        }

        // Draw the button
        if (GUI.Button(position, buttonText))
        {
            if (!hasParameters)
            {
                // Invoke the method
                if (method.IsStatic)
                {
                    method.Invoke(null, null);
                }
                else
                {
                    method.Invoke(target, null);
                }
            }
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;
        return buttonAttribute.ButtonHeight;
    }
}
#endif