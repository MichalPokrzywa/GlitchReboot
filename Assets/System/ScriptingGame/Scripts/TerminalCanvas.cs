using System.Collections.Generic;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalCanvas : MonoBehaviour
{
    [SerializeField] GameObject blocksPanel;

    public TMP_Text codeText;
    public GameObject sideNumbers;
    [HideInInspector] public Image passImage;
    public bool showRobot = false;

    bool blocksPanelOn = false;

    public void Start()
    {
        sideNumbers.SetActive(showRobot);
    }

    public void OnSwitchButtonClicked()
    {
        blocksPanelOn = !blocksPanelOn;
        blocksPanel.SetActive(blocksPanelOn);
    }

    public void SetCodeText(string code)
    {
        //string cleanedCode = Regex.Replace(code, @"\b\w+:", "").Replace("\t", "   ");
        if(showRobot) codeText.text = code;
        sideNumbers.SetActive(showRobot);
    }
    public void SetNeutralText(string neutral)
    {
        //string cleanedCode = Regex.Replace(code, @"\b\w+:", "").Replace("\t", "   ");
        if(!showRobot) codeText.text = neutral;
        sideNumbers.SetActive(showRobot);
    }
    public void ChangeTextType(string robot, string human)
    {
        showRobot = !showRobot;
        sideNumbers.SetActive(showRobot);
        codeText.text = showRobot ? robot : human;
    }
    public void SetImageColor(bool pass)
    {
        passImage.color = pass ? Color.green : Color.red;
    }
}
