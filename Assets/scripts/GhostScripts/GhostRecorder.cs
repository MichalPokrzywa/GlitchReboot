using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public List<TimeIndicatorHandler> indicatorHandlers;
    public List<GameObject> markerPoints;
    private bool isHeld = false;
    private float timer;
    private float timeValue;
    private float[] buttonHoldTimers = new float[4];

    Vector3 playerPosition;
    Vector2 playerRotation;

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        if (Input.GetKeyUp(KeyCode.Alpha1)) OnButtonPressed(0);
        if (Input.GetKeyUp(KeyCode.Alpha2)) OnButtonPressed(1);
        if (Input.GetKeyUp(KeyCode.Alpha3)) OnButtonPressed(2);
        if (Input.GetKeyUp(KeyCode.Alpha4)) OnButtonPressed(3);

        HandleButtonHold(KeyCode.Alpha1, 0, () => OnButtonHeld(0));
        HandleButtonHold(KeyCode.Alpha2, 1, () => OnButtonHeld(1));
        HandleButtonHold(KeyCode.Alpha3, 2, () => OnButtonHeld(2));
        HandleButtonHold(KeyCode.Alpha4, 3, () => OnButtonHeld(3));
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

    void StartRecording(GameObject ghost)
    {
        var ghostController = ghost.gameObject.GetComponent<MovementController>();
        if (ghostController == null)
            return;

        var ghostInput = ghostController.Input as GhostInput;
        if (ghostInput == null)
            return;

        var playerRotationController = gameObject.GetComponent<RotationController>();
        var ghostRotationController = ghost.gameObject.GetComponent<RotationController>();

        switch (ghostInput.GhostMovementState)
        {
            case GhostInput.State.NONE:
                ghostController.gameObject.SetActive(true);
                playerPosition = transform.position;
                if (playerRotationController != null)
                {
                    playerRotation.x = playerRotationController.xRotation;
                    playerRotation.y = playerRotationController.yRotation;
                }
                ghostInput.SetState(GhostInput.State.RECORDING);
                Debug.Log("Recording");
                break;
            case GhostInput.State.RECORDING:
                ghostInput.SetState(GhostInput.State.RECORDED);
                Debug.Log("Recorded");
                break;
            case GhostInput.State.RECORDED:
                ghostController.gameObject.transform.position = playerPosition + new Vector3(0, 0.5f, 0);
                if (ghostRotationController != null && playerRotationController != null)
                {
                    ghostRotationController.xRotation = playerRotation.x;
                    ghostRotationController.yRotation = playerRotation.y;
                }
                ghostInput.SetState(GhostInput.State.REPLAY);
                Debug.Log("Replay");
                break;
            case GhostInput.State.REPLAY:
                ghostInput.SetState(GhostInput.State.NONE);
                Debug.Log("None");
                break;
        }
    }

    void OnButtonPressed(int buttonNumber)
    {
        if (isHeld)
        {
            isHeld = false;
            return;
        }

        StartRecording(gameObjects[buttonNumber]);
    }

    void OnButtonHeld(int buttonNumber)
    {
        var ghostController = gameObjects[buttonNumber].GetComponent<MovementController>();
        if (ghostController == null)
            return;

        ghostController.Input.ResetState();
    }
}
