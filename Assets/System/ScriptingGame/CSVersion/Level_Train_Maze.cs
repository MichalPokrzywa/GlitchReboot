using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Level_Train_Maze : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject waypoint;

    public GameObject mazeParrent;
    public float lowerBy = 3.2f;

    private bool isRed;
    private Vector3 mazeBasicTransform;

    protected override void Start()
    {
        base.Start();
        mazeBasicTransform = mazeParrent.transform.position;
    }
    public override void DoTerminalCode()
    {
        isRed = GetVariableValue<bool>("isRed");

        float targetY = isRed ? mazeBasicTransform.y : mazeBasicTransform.y - lowerBy;

        mazeParrent.transform.DOMoveY(targetY, 0.5f) // czas animacji: 0.5s
            .SetEase(Ease.InOutSine);
    }

}