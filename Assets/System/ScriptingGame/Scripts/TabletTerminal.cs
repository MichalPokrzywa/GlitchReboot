using TMPro;
using UnityEngine;

public class TabletTerminal : Singleton<TabletTerminal>
{
    [SerializeField] GameObject blocksPanel;

    public TMP_Text codeText;
    public GameObject sideNumbers;
    public PuzzleBase assignedTerminal;
    private string humanCode;
    private string robotCode;
    public bool showRobot = false;

    private bool blocksPanelOn = false;

    void Awake()
    {
        blocksPanel.SetActive(false);
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
        if (terminal != null && terminal != assignedTerminal)
        {
            if (assignedTerminal != null)
            {
                assignedTerminal.onCodeUpdate.RemoveListener(Instance.UpdateTerminal);
            }
            assignedTerminal = terminal;
            assignedTerminal.onCodeUpdate.AddListener(Instance.UpdateTerminal);

        }
    }

    public void OnSwitchButtonClicked()
    {
        blocksPanelOn = !blocksPanelOn;
        blocksPanel.SetActive(blocksPanelOn);
    }
}
