using UnityEngine;
using DG.Tweening;
public class LevelTrainSection3 : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject launcher1;
    public GameObject launcher2;
    public GameObject launcher3;

    private int pushValue1;
    private int pushValue2;
    private int pushValue3;
    private Vector3 launcher1StartPos;
    private Vector3 launcher2StartPos;
    private Vector3 launcher3StartPos;
    [SerializeField] private GameObject elevator;

    protected override void Start()
    {
        base.Start();
        launcher1StartPos = launcher1.transform.position;
        launcher2StartPos = launcher2.transform.position;
        launcher3StartPos = launcher3.transform.position;
    }

    public override void DoTerminalCode()
    {
        pushValue1 = GetVariableValue<int>("pushValueX");
        pushValue2 = GetVariableValue<int>("pushValueY");
        pushValue3 = GetVariableValue<int>("pushValueZ");

        float targetX = launcher1StartPos.x + pushValue1;
        float targety = launcher2StartPos.y + pushValue2;
        float targetz = launcher3StartPos.z + pushValue3;

        launcher1.transform.DOMoveX(targetX, 0.5f)
            .SetEase(Ease.InOutSine);
        launcher2.transform.DOMoveY(targety, 0.5f)
            .SetEase(Ease.InOutSine);
        launcher3.transform.DOMoveZ(targetz, 0.5f)
            .SetEase(Ease.InOutSine);

        if (GetVariableValue<bool>("Elevator"))
            elevator.GetComponent<LevelExitElevator>().OpenDoors();
    }
}