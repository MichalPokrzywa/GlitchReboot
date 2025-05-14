using DG.Tweening;
using UnityEngine;

public class LevelExitElevator : MonoBehaviour
{
    [Header("Select Next Level")] 
    public Scene nextScene;

    [Header("References")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Shake Settings")]
    public float moveShakeDuration = 3f;     // Duration of shake when 'moving'
    public float moveShakeStrength = 0.2f;   // Strength of shake when 'moving'
    public float startShakeDuration = 0.3f;   // Duration of shake at stop
    public float startShakeStrength = 0.1f;   // Strength of shake at stop

    [Header("Door Settings")]
    public float doorCloseDuration = 0.5f;       // Time to close doors
    public Vector3 doorLeftCloseOffset = new Vector3(-1f, 0f, 0f);
    public Vector3 doorRightCloseOffset = new Vector3(1f, 0f, 0f);

    private Vector3 doorLeftOpenPos;
    private Vector3 doorRightOpenPos;
    private bool doorsOpen = false;

    void Start()
    {
        doorLeftOpenPos = leftDoor.localPosition;
        doorRightOpenPos = rightDoor.localPosition;
    }
    void CloseDoors()
    {
        Sequence seq = DOTween.Sequence();

        // 1. Close Doors
        seq.Append(
                DOTween.Sequence()
                    .Join(leftDoor.DOLocalMove(doorLeftOpenPos + doorLeftCloseOffset, doorCloseDuration).SetEase(Ease.InQuad))
                    .Join(rightDoor.DOLocalMove(doorRightOpenPos + doorRightCloseOffset, doorCloseDuration).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        doorsOpen = false;
                        Debug.Log("Doors closed after player Entered elevator.");
                    })
        );
        //2. Small delay
        seq.AppendInterval(1f);

        //3. Move Elevator
        seq.Append(
            DOTween.Sequence()
                .Append(Camera.main.transform.DOShakePosition(startShakeDuration, startShakeStrength, randomnessMode: ShakeRandomnessMode.Harmonic))
                .Append(Camera.main.transform.DOShakePosition(moveShakeDuration, moveShakeStrength, randomnessMode: ShakeRandomnessMode.Harmonic))
                .InsertCallback(2f, () =>
                {
                    DependencyManager.sceneLoader.LoadScene(nextScene);
                })

                .OnComplete(() =>
                {
                    Debug.Log("Sequence complete");
                })
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (doorsOpen) return;
        if (other.CompareTag("Player"))
        {
            CloseDoors();
        }
    }

}
