using DG.Tweening;
using UnityEngine;

public class TutorialLevel1 : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject doors;

    public override void DoTerminalCode()
    {
        string x = GetVariableValue<string>("value_x");
        doors.transform.DORotate(x == "Open" ? new Vector3(0, 90, 0) : Vector3.zero, 0.3f);
    }
}