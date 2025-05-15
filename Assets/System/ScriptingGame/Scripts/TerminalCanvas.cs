using System.Collections.Generic;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalCanvas : MonoBehaviour
{
    public TMP_Text codeText;
    public TMP_Text neutralText;
    public Image passImage;

    public void SetCodeText(string code)
    {
        //prepareText();
        //string cleanedCode = Regex.Replace(code, @"\b\w+:", "").Replace("\t", "   ");
        codeText.text = code;
    }
    public void SetNeutralText(string neutral)
    {
        //prepareText();
        //string cleanedCode = Regex.Replace(code, @"\b\w+:", "").Replace("\t", "   ");
        neutralText.text = neutral;
    }

    public void SetImageColor(bool pass)
    {
        passImage.color = pass ? Color.green : Color.red;
    }
}
