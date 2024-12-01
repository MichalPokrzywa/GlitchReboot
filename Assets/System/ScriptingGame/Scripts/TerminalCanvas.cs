using System.Collections.Generic;
using MoonSharp.Interpreter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalCanvas : MonoBehaviour
{
    public TMP_Text codeText;
    public TMP_Text variableText;
    public Image passImage;
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
        //debug
/*        foreach (var pair in variableTable.Pairs)
        {
            //Debug.Log(variableTable[pair.variableName]);
            variableText.text += $"{pair.Key} = {variableTable[pair.Value] ?? "null"}";
            variableText.text += "\n";
        }*/
    }

    public void SetImageColor(bool pass)
    {
        passImage.color = pass ? Color.green : Color.red;
    }

}
