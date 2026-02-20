using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class HideIfAttribute : PropertyAttribute
{
    public string ConditionFieldName { get; private set; }
    public object ExpectedValue { get; private set; }

    // Constructor for bool (backward compatible)
    public HideIfAttribute(string conditionFieldName, bool expectedValue = true)
    {
        ConditionFieldName = conditionFieldName;
        ExpectedValue = expectedValue;
    }

    // Constructor for enum and other types
    public HideIfAttribute(string conditionFieldName, object expectedValue)
    {
        ConditionFieldName = conditionFieldName;
        ExpectedValue = expectedValue;
    }
}