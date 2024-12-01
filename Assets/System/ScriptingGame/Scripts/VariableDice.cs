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
    private AudioSource audioSource;

    private IVariableTypeHandler handler;

    void Start()
    {
        InitializeHandler();
        audioSource = GetComponent<AudioSource>();
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
    private void OnTriggerEnter(Collider other)
    {
        audioSource.Play();
        // Check if the other object is on the correct layer
        if (other.gameObject.layer == LayerMask.NameToLayer("VariablePlatform"))
        {
            // Check if the other object has a VariablePlatform component
            VariablePlatform platform = other.gameObject.GetComponent<VariablePlatform>();
            if (platform != null)
            {
                // Get the dice's current value and send it to the platform
                object currentValue = handler?.GetValue();
                platform.ReceiveValue(currentValue);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the other object is on the correct layer
        if (other.gameObject.layer == LayerMask.NameToLayer("VariablePlatform"))
        {
            // Check if the other object has a VariablePlatform component
            VariablePlatform platform = other.gameObject.GetComponent<VariablePlatform>();
            if (platform != null)
            {
                // Clear the value on the platform when the trigger is exited
                platform.ClearValue();
            }
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
