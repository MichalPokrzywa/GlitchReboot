using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class VariablePlatform : MonoBehaviour
{
    [SerializeField] 
    public string variableName = "x_value";
    public VariableType type;

    [HideInInspector] public UnityEvent<string,object> variableAdded;
    [HideInInspector] public UnityEvent<string> variableRemoved;

    public List<TMP_Text> textList;

    void Start()
    {
        foreach (TMP_Text text in textList)
        {
            text.text = variableName;
        }
    }
    public void ReceiveValue(object value)
    {
        // Handle the received value
        if (value is int intValue && type == VariableType.Number)
        {
            Debug.Log($"Platform received integer: {intValue}");
            variableAdded.Invoke(variableName,intValue);
            UpdateText(true);
        }
        else if (value is bool boolValue && type == VariableType.Boolean)
        {
            Debug.Log($"Platform received boolean: {boolValue}");
            variableAdded.Invoke(variableName, boolValue);
            UpdateText(true);
        }
        else
        {
            Debug.LogError("Platform received unknown value type");
        }
    }
    public void ClearValue()
    {
        // Clear the value stored on the platform
        //currentValue = null;
        variableRemoved.Invoke(variableName);
        Debug.Log("Platform value cleared");
        UpdateText(false);
    }
    private void UpdateText(bool isOn)
    {
        foreach (TMP_Text text in textList)
        {
            text.color = isOn ? Color.green : Color.grey;
        }
    }
}
