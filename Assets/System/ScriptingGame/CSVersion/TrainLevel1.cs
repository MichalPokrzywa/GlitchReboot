using System.Collections.Generic;
using UnityEngine;

public class TrainLevel1 : PuzzleBase
{
    [Header("PuzzleItems")]
    public List<GameObject> turrets = new List<GameObject>();

    public override void DoTerminalCode()
    {
        foreach (GameObject t in turrets)
        {
            Debug.Log(t.transform.GetChild(0));
            t.GetComponent<CannonController>().selectedStrengthLevel = GetVariableValue<string>("fireForce");
            if (GetVariableValue<int>("angle") == 0)
            {
                t.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                t.transform.GetChild(0).localRotation = Quaternion.Euler(-GetVariableValue<int>("angle"), -90f, 0f);
            }
        }
    }
}