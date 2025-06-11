using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
using System.Collections;

public class ChessLevelScript : PuzzleBase
{
    private bool solved = false;
    [Header("ConnectedObject")]
    public GameObject moveableObject;

    [Header("Pawn")]
    public GameObject pawn;
    [Header("Knight")]
    public GameObject knight;
    [Header("Bishop")]
    public GameObject bishop;
    [Header("Rook")]
    public GameObject rook;
    [Header("Queen")]
    public GameObject queen;
    [Header("King")]
    public GameObject king;
    public override void DoTerminalCode()
    {
        if (solved) return;
        string actualPiece = "0";
        if (GetVariableValue<string>("Pawn") != "0")
            actualPiece = "Pawn";
        if (GetVariableValue<string>("Knight") != "0")
            actualPiece = "Knight";
        if (GetVariableValue<string>("Bishop") != "0")
            actualPiece = "Bishop";
        if (GetVariableValue<string>("Rook") != "0")
            actualPiece = "Rook";
        if (GetVariableValue<string>("Queen") != "0")
            actualPiece = "Queen";
        if (GetVariableValue<string>("King") != "0")
            actualPiece = "King";
        string square = GetVariableValue<string>(actualPiece);
        Debug.Log(actualPiece);
        Debug.Log(square);
       

        if (actualPiece == "Bishop" && square == "D5")
        {
            Debug.Log("Dobrze!");
            solved = true;
            Vector3 newPosition = moveableObject.transform.position + new Vector3(-15, 0, 15);
            moveableObject.transform.DOMove(newPosition, 5, false);
        }
        else if(actualPiece != "0")
        {
            StartCoroutine(DelayedDeactivate(actualPiece, 1.0f));            
        }
    }
    private IEnumerator DelayedDeactivate(String pieceName, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (pieceName != "Bishop")
        {
            GameObject platformObject = allLevelPlatforms.Find((platform => platform.GetHandler().VariableName == pieceName)).gameObject;
            platformObject.transform.position = new Vector3(0.0f,-100.0f,0.0f);
            switch (pieceName)
            {
                case "Pawn":
                    {
                        pawn.SetActive(false);
                        break;
                    }
                case "Knight":
                    {
                        knight.SetActive(false);
                        break;
                    }
                case "Rook":
                    {
                        rook.SetActive(false);
                        break;
                    }
                case "Queen":
                    {
                        queen.SetActive(false);
                        break;
                    }
                case "King":
                    {
                        king.SetActive(false);
                        break;
                    }
            }

        }
    }
}

