using UnityEngine;

public class Condition : MonoBehaviour
{
    [SerializeField] TerminalScript terminalScript;
    public bool returnCondition()
    {
        Debug.LogWarning("Warunek: " + terminalScript.result);
        return terminalScript.result;
    }
}
