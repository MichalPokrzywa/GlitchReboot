using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
public class ChessLevelScript : PuzzleBase
{
    private bool once = true;
    [Header("ConnectedObject")]
    public GameObject moveableObject;
    public override void DoTerminalCode()
    {
        string piece = GetVariableValue<string>("Figura"); 
        string place = GetVariableValue<string>("Pole");
        if (piece == "Goniec" && place == "D5" && once)
        {
            Vector3 newPosition = moveableObject.transform.position + new Vector3(-15, 0, 15);
            //moveableObject.transform.position = newPosition;
            moveableObject.transform.DOMove(newPosition, 5, false);
            once = false;
        }
        else
        {
            Debug.Log("Zly ruch");
        }
    }
}

