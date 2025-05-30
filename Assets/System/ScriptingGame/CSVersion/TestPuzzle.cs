using UnityEngine;

public class TestPuzzle : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject waypoint;
    public override void DoTerminalCode()
    {
        int x = GetVariableValue<int>("value_x");
        Vector3 newPosition = transform.position + new Vector3(x, 0, 0);
       // waypoint.transform.position = newPosition;
    }
}
