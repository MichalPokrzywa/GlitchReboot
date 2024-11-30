using UnityEngine;
using UnityEngine.Events;

public class VariablePlatform : MonoBehaviour
{
    [SerializeField] 
    public string variableName = "x_value";
    public VariableType type;

   [HideInInspector] public UnityEvent<string,object> variableAdded;
   [HideInInspector] public UnityEvent<string> variableRemoved;

    public void ReceiveValue(object value)
    {
        // Handle the received value
        if (value is int intValue && type == VariableType.Number)
        {
            Debug.Log($"Platform received integer: {intValue}");
            variableAdded.Invoke(variableName,intValue);
        }
        else if (value is bool boolValue && type == VariableType.Boolean)
        {
            Debug.Log($"Platform received boolean: {boolValue}");
            variableAdded.Invoke(variableName, boolValue);
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
        Debug.Log("Platform value cleared");
    }
}
