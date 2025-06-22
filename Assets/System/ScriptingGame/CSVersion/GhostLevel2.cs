using System;
using UnityEngine;

public class GhostLevel2 : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject elevator;
    private bool isActive = false;

    public override void DoTerminalCode()
    {
        if(GetVariableValue<string>("objectName") =="ExitElevator" && !GetVariableValue<bool>("isClosed") && isActive)
        {
            elevator.GetComponent<LevelExitElevator>().OpenDoors();
        }
    }

    public void SetActive()
    {
        isActive = !isActive;
    }
}