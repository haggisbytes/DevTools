using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{
    public string ButtonText { get; private set; }
    public float ButtonHeight { get; private set; }

    public ButtonAttribute()
    {
        ButtonText = null; // Will use method name
        ButtonHeight = 20f;
    }

    public ButtonAttribute(string buttonText)
    {
        ButtonText = buttonText;
        ButtonHeight = 20f;
    }

    public ButtonAttribute(string buttonText, float height)
    {
        ButtonText = buttonText;
        ButtonHeight = height;
    }
}