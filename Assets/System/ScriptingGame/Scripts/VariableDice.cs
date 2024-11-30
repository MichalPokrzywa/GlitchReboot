using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VariableDice : MonoBehaviour
{
    public VariableType type;
    public List<TMP_Text> textList;

    [SerializeField] private int baseNumberValue = 1; // Initial value for Number
    [SerializeField] private bool baseBooleanValue = false; // Initial value for Boolean

    private IVariableTypeHandler handler;

    void Start()
    {
        InitializeHandler();
    }

    void InitializeHandler()
    {
        switch (type)
        {
            case VariableType.Number:
                handler = new NumberHandler(baseNumberValue);
                UpdateValue(baseNumberValue);
                break;
            case VariableType.Boolean:
                handler = new BooleanHandler(baseBooleanValue);
                UpdateValue(baseBooleanValue);
                break;
        }
    }

    public object GetCurrentValue()
    {
        if (handler == null)
        {
            Debug.LogWarning("Handler not initialized!");
            return null;
        }

        return handler.GetValue();
    }

    public void UpdateValue(object value)
    {
        handler.UpdateValue(value);
        foreach (TMP_Text text in textList)
        {
            handler.UpdateTextValue(text);
        }
    }


}
public interface IVariableTypeHandler
{
    void UpdateValue(object value);
    void UpdateTextValue(TMP_Text text);
    object GetValue(); // Returns the current value
}

public class NumberHandler : IVariableTypeHandler
{
    private int currentValue;
    public NumberHandler(int initialValue)
    {
        currentValue = initialValue;
    }
    public void UpdateValue(object value)
    {
        currentValue = Convert.ToInt32(value);
    }

    public void UpdateTextValue(TMP_Text text)
    {
        text.text = currentValue.ToString();
    }

    public object GetValue()
    {
        return currentValue;
    }
}
public class BooleanHandler : IVariableTypeHandler
{
    private bool currentValue;

    public BooleanHandler(bool initialValue)
    {
        currentValue = initialValue;
    }

    public void UpdateValue(object value)
    {
        currentValue = Convert.ToBoolean(value);
    }

    public void UpdateTextValue(TMP_Text text)
    {
        text.text = currentValue ? "True" : "False";
    }


    public object GetValue()
    {
        return currentValue;
    }
}

public enum VariableType
{
    Number,
    Boolean
}
