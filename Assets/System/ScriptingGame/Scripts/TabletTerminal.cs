using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TabletTerminal : Singleton<TabletTerminal>
{
    [Header("UI References")]
    [SerializeField] GameObject blocksPanel;
    [SerializeField] TMP_Text codeText;
    [SerializeField] GameObject sideNumbers;
    [SerializeField] TMP_Text terminalNameText;

    [Header("Currently assigned terminal")]
    public PuzzleBase assignedTerminal;
    string humanCode;
    string robotCode;

    bool showRobot = false;
    bool blocksPanelOn = false;

    List<PuzzleBase> copiedTerminals = new List<PuzzleBase>();
    int terminalIndex = 0;

    void Awake()
    {
        if (blocksPanel != null)
            blocksPanel.SetActive(false);
    }

    void ChangeTerminalIndex(int direction)
    {
        if (copiedTerminals == null || copiedTerminals.Count == 0)
            return;

        terminalIndex = (terminalIndex + direction + copiedTerminals.Count) % copiedTerminals.Count;
        DoAssignTerminal(copiedTerminals[terminalIndex]);
    }

    void DoAssignTerminal(PuzzleBase terminal)
    {
        if (assignedTerminal != null)
        {
            assignedTerminal.onCodeUpdate.RemoveListener(Instance.UpdateTerminal);
        }

        assignedTerminal = terminal;
        assignedTerminal.onCodeUpdate.AddListener(Instance.UpdateTerminal);
        terminalNameText.text = assignedTerminal.name;
    }

    public void SendText(string robot,string human)
    {
        humanCode = human;
        robotCode = robot;
        sideNumbers.SetActive(showRobot);
        codeText.text = showRobot ? robotCode : humanCode;
    }

    public void UpdateTerminal(string robot, string human)
    {
        humanCode = human;
        robotCode = robot;
        sideNumbers.SetActive(showRobot);
        codeText.text = showRobot ? robotCode : humanCode;
    }

    public void ChangeTextType()
    {
        showRobot = !showRobot;
        sideNumbers.SetActive(showRobot);
        codeText.text = showRobot ? robotCode : humanCode;
    }

    public void AssignTerminal(PuzzleBase terminal)
    {
        if (terminal == null)
            return;

        // if the terminal hasn't already been assigned, add it to the HashSet
        if (!copiedTerminals.Contains(terminal))
            copiedTerminals.Add(terminal);

        DoAssignTerminal(terminal);
    }

    #region ButtonUnityEvents

    public void OnSwitchButtonClicked()
    {
        blocksPanelOn = !blocksPanelOn;
        blocksPanel.SetActive(blocksPanelOn);
    }

    public void OnNextTerminalButtonClicked()
    {
        ChangeTerminalIndex(1);
    }

    public void OnPreviousTerminalButtonClicked()
    {
        ChangeTerminalIndex(-1);
    }

    #endregion
}
