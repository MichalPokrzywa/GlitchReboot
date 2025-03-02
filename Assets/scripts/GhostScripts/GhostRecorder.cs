using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    [SerializeField] List<GameObject> ghosts;
    [SerializeField] List<TimeIndicatorHandler> indicatorHandlers;
    [SerializeField] List<GameObject> markerPoints;

    bool isHeld = false;
    float[] buttonHoldTimers = new float[4];

    public enum GhostKey
    {
        Ghost1 = KeyCode.Alpha1,
        Ghost2 = KeyCode.Alpha2,
        Ghost3 = KeyCode.Alpha3,
        Ghost4 = KeyCode.Alpha4
    }

    void Update()
    {
        CheckButtonPress(GhostKey.Ghost1, 0);
        CheckButtonPress(GhostKey.Ghost2, 1);
        CheckButtonPress(GhostKey.Ghost3, 2);
        CheckButtonPress(GhostKey.Ghost4, 3);
    }

    void CheckButtonPress(GhostKey ghostKey, int index)
    {
        if (Input.GetKeyUp((KeyCode)ghostKey))
            OnButtonPressed(index);

        HandleButtonHold((KeyCode)ghostKey, index, () => OnButtonHeld(index));
    }

    void HandleButtonHold(KeyCode key, int index, System.Action onHoldAction)
    {
        if (Input.GetKey(key))
        {
            buttonHoldTimers[index] += Time.deltaTime;
            float holdTime = 2f;
            if (buttonHoldTimers[index] >= holdTime)
            {
                isHeld = true;
                onHoldAction?.Invoke();
                buttonHoldTimers[index] = 0f;
            }
        }
        else
        {
            buttonHoldTimers[index] = 0f;
        }
    }

    void OnButtonPressed(int buttonNumber)
    {
        if (isHeld)
        {
            isHeld = false;
            return;
        }

        HandleGhostRecording(ghosts[buttonNumber]);
    }

    void OnButtonHeld(int buttonNumber)
    {
        var ghostController = ghosts[buttonNumber].GetComponent<MovementController>();
        if (ghostController == null)
            return;

        ghostController.Input.ResetState();
    }

    void HandleGhostRecording(GameObject ghost)
    {
        var ghostController = ghost.GetComponent<MovementController>();
        if (ghostController == null)
            return;

        var ghostInput = ghostController.Input as GhostInput;
        if (ghostInput == null)
            return;

        var playerRotationController = gameObject.GetComponent<RotationController>();
        if (playerRotationController == null)
            return;

        switch (ghostInput.GhostMovementState)
        {
            // --- START RECORDING ---
            case GhostInput.State.NONE:
                // save starting position and rotation
                ghostInput.SetReplayStartingPosition(transform.position, new Vector2(playerRotationController.xRotation, playerRotationController.yRotation));
                ghostInput.SetState(GhostInput.State.RECORDING);
                Debug.Log("Recording");
                break;
            // --- END RECORDING ---
            case GhostInput.State.RECORDING:
                ghostInput.SetState(GhostInput.State.RECORDED);
                Debug.Log("Recorded");
                break;
            // --- REPLAY ---
            case GhostInput.State.RECORDED or GhostInput.State.REPLAY:
                ghostInput.PrepareForReplay();
                ghostInput.SetState(GhostInput.State.REPLAY);
                Debug.Log("Replay");
                break;
        }
    }


}
