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
        //prepareText();
        codeText.text = code;
    }

    public void SetVariableText(Table variableTable,List<VariablePlatform> list)
    {
        //prepareText();
        variableText.text = "";
        foreach (var pair in list)
        {
            Debug.Log(variableTable[pair.variableName]);
            variableText.text += $"{pair.variableName} = {variableTable[pair.variableName] ?? "null"}";
            variableText.text += "\n";
        }
        /*
        foreach (var pair in variableTable.Pairs)
        {
            //Debug.Log(variableTable[pair.variableName]);
            variableText.text += $"{pair.Key} = {variableTable[pair.Value] ?? "null"}";
            variableText.text += "\n";
        }
        */
    }

    public void SetImageColor(bool pass)
    {
        passImage.color = pass ? Color.green : Color.red;
    }

    private void prepareText()
    {
        TMP_Text result = new TextMeshPro();
        result.text = "";
        bool comment = false;
        for (int i = 0; i < codeText.text.Length; i++)
        {
            if (codeText.text[i] == '-' && i + 1< codeText.text.Length 
                && codeText.text[i + 1] == '-')
            {
                if (!comment)
                {
                    comment = true;
                  
                } else
                {
                    comment = false;
                    i ++;
                    continue;
                }
            }
            if (!comment)
                result.text = result.text + codeText.text[i];
        }
        if (result.text[result.text.Length - 2] != '(')
            result.text = result.text + "   door_loop()";      
        codeText.text = result.text;
        
    }
}
