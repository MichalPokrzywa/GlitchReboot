using System;
using UnityEngine;

public class GhostLevel2 : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject waypoint;
    private bool isActive = false;

    public override void DoTerminalCode()
    {
        GameObject elevator = GameObject.Find("ExitElevator");
        if(GetVariableValue<String>("ExitElevator")=="ExitElevator" && !GetVariableValue<bool>("isClosed") && isActive)
        {
            Debug.Log("jestem tuu w mroku");
            elevator.GetComponent<LevelExitElevator>().OpenDoors();
        }
    }

    public void SetActive()
    {
        isActive = !isActive;
    }
}