using UnityEngine;
using DG.Tweening;

public class LevelTrainSection3 : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject launcher;

    private int pushValue;
    private Vector3 launcherStartPos;

    protected override void Start()
    {
        base.Start();
        launcherStartPos = launcher.transform.position;
    }

    public override void DoTerminalCode()
    {
        pushValue = GetVariableValue<int>("pushValue");

        float targetX = launcherStartPos.x + pushValue;

        launcher.transform.DOMoveX(targetX, 0.5f)
            .SetEase(Ease.InOutSine);
    }
}