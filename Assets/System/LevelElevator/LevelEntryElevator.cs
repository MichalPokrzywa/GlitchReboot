using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LevelEntryElevator : MonoBehaviour
{
    [Header("References")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform spawnPoint;

    [Header("Shake Settings")]
    public float moveShakeDuration = 3f;     // Duration of shake when 'moving'
    public float moveShakeStrength = 0.2f;   // Strength of shake when 'moving'
    public float stopShakeDuration = 0.5f;   // Duration of shake at stop
    public float stopShakeStrength = 0.1f;   // Strength of shake at stop

    [Header("Door Settings")]
    public float doorOpenDuration = 1f;        // Time to open doors
    public float doorCloseDuration = 0.5f;       // Time to close doors
    public Vector3 doorLeftOpenOffset = new Vector3(-1f, 0f, 0f);
    public Vector3 doorRightOpenOffset = new Vector3(1f, 0f, 0f);

    private Vector3 doorLeftClosedPos;
    private Vector3 doorRightClosedPos;
    private bool doorsOpen = false;
    void Awake()
    {
        DependencyManager.sceneLoader?.sceneLoaded.AddListener(SetupPlayer);
    }
    void SetupPlayer()
    {
        // Cache closed door positions
        doorLeftClosedPos = leftDoor.localPosition;
        doorRightClosedPos = rightDoor.localPosition;
        GameObject player = FindAnyObjectByType<Interactor>().gameObject;
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        PlayEffectsSequence();
    }

    void PlayEffectsSequence()
    {
        Sequence seq = DOTween.Sequence();

        // 1. Shake camera to simulate elevator moving
        seq.Append(Camera.main.transform.DOShakePosition(moveShakeDuration, moveShakeStrength,randomnessMode: ShakeRandomnessMode.Harmonic));

        // 2. Brief shake on stop
        seq.Append(Camera.main.transform.DOShakePosition(stopShakeDuration, stopShakeStrength,randomnessMode: ShakeRandomnessMode.Harmonic));

        // 3. Open doors
        seq.Append(
            DOTween.Sequence()
                .Join(leftDoor.DOLocalMove(doorLeftClosedPos + doorLeftOpenOffset, doorOpenDuration).SetEase(Ease.OutQuad))
                .Join(rightDoor.DOLocalMove(doorRightClosedPos + doorRightOpenOffset, doorOpenDuration).SetEase(Ease.OutQuad))
        );

        seq.OnComplete(() =>
        {
            doorsOpen = true;
            Debug.Log("Camera shake & door sequence complete. Doors opened.");
        });
    }
    void OnTriggerExit(Collider other)
    {
        if (!doorsOpen) return;

        if (other.CompareTag("Player"))
        {
            CloseDoors();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player is in Trigger");
    }

    void CloseDoors()
    {
        DOTween.Sequence()
            .Join(leftDoor.DOLocalMove(doorLeftClosedPos, doorCloseDuration).SetEase(Ease.InQuad))
            .Join(rightDoor.DOLocalMove(doorRightClosedPos, doorCloseDuration).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                doorsOpen = false;
                Debug.Log("Doors closed after player exited elevator.");
            });
    }
}
