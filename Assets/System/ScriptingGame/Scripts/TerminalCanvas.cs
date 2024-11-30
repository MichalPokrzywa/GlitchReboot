using System.Collections.Generic;
using MoonSharp.Interpreter;
using TMPro;
using UnityEngine;

public class TerminalCanvas : MonoBehaviour
{
    public TMP_Text codeText;
    public TMP_Text variableText;

    public void SetCodeText(string code)
    {
        codeText.text = code;
    }

    public void SetVariableText(Table variableTable,List<VariablePlatform> list)
    {
        variableText.text = "";
        foreach (var pair in list)
        {
            Debug.Log(variableTable[pair.variableName]);
            variableText.text += $"{pair.variableName} = {variableTable[pair.variableName] ?? "null"}";
            variableText.text += "\n";
        }
    }

}
