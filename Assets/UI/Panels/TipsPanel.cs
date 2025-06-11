using System;
using TMPro;
using UnityEngine;

public class TipsPanel : Panel
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] string diceThrowTip = "Press [E] to throw the dice";
    [SerializeField] string terminalTip = "Press [Q] to open the terminal";
    [SerializeField] string terminalLanguageChangeTip = "Press [E] to change language in the terminal";

    public enum eTipType
    {
        None,
        DiceThrow,
        Terminal,
        TerminalLanguageChange
    }

    public void SetText(eTipType tipType)
    {
        switch (tipType)
        {
            case eTipType.DiceThrow:
                text.text = diceThrowTip;
                break;
            case eTipType.Terminal:
                text.text = terminalTip;
                break;
            case eTipType.TerminalLanguageChange:
                text.text = terminalLanguageChangeTip;
                break;
            default:
                text.text = string.Empty;
                break;
        }
    }

    public override void Close(Action onComplete = null)
    {
        base.Close();
    }

    public override void Open(Action onComplete = null)
    {
        base.Open();
    }
}