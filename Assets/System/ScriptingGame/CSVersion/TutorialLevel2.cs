using DG.Tweening;
using UnityEngine;

public class TutorialLevel2 : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject stairs;

    private float basePosition;


    protected override void Start()
    {
        base.Start();
        basePosition = stairs.transform.position.y;
    }

    public override void DoTerminalCode()
    {
        int position = GetVariableValue<int>("Numeric");
        bool show = GetVariableValue<bool>("Logical");
        string name = GetVariableValue<string>("Text");

        if (name == "Stairs")
        {
            //TODO: Change it to stopping glitching 
            if (!show)
            {
                stairs.transform.DOMoveY(1-position,1f);
            }
        }
        else
        {
            stairs.SetActive(show);
            stairs.transform.DOMoveY(1, 1f);
        }
    }
}