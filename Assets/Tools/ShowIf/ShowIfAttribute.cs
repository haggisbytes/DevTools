// ConditionalFieldAttribute.cs - The main attribute
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ShowIfAttribute : PropertyAttribute
{
    public string ConditionFieldName { get; private set; }
    public object ExpectedValue { get; private set; }

    public ShowIfAttribute(string conditionFieldName, bool expectedValue = true)
    {
        ConditionFieldName = conditionFieldName;
        ExpectedValue = expectedValue;
    }
    public ShowIfAttribute(string conditionFieldName, object expectedValue)
    {
        ConditionFieldName = conditionFieldName;
        ExpectedValue = expectedValue;
    }
}