using Unity.VisualScripting;
using UnityEngine;

public class Level_Train_Maze : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject waypoint;

    public GameObject mazeParrent;
    public float lowerBy = 3.0f;

    private bool isRed;
    private Vector3 mazeBasicTransform;

    protected override void Start()
    {
        base.Start();
        mazeBasicTransform = mazeParrent.transform.position;
    }
    public override void DoTerminalCode()
    {
        Debug.Log("This is Empty");
        isRed = GetVariableValue<bool>("isRed");
        if (isRed)
        {
            mazeParrent.transform.position -= new Vector3(0, lowerBy, 0);
        }
        if (!isRed)
        {
            mazeParrent.transform.position = mazeBasicTransform;
        }
    }
}