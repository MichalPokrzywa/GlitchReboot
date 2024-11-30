using UnityEngine;

public class Condition : MonoBehaviour
{
    [SerializeField] TerminalScript terminalScript;
    public bool returnCondition()
    {
        return terminalScript.result;
    }
}
