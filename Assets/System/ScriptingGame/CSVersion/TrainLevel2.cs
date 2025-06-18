using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TrainLevel2 : PuzzleBase
{
    [Header("PuzzleItems")]
    [SerializeField] private List<GameObject> stations = new List<GameObject>();
    [SerializeField] private Transform Train;

    public override void DoTerminalCode()
    {
        if (GetVariableValue<bool>("Station1"))
        {
            stations[0].SetActive(true);
        }
        else if (GetVariableValue<bool>("Station2"))
        {
            stations[1].SetActive(true);
        }
        else if (GetVariableValue<bool>("Station3"))
        {
            stations[2].SetActive(true);
        }
        else if(GetVariableValue<bool>("Station4"))
        {
            stations[3].SetActive(true);
        }
        Train.GetChild(0).GetComponent<TrainController>().drive = true;
    }
}