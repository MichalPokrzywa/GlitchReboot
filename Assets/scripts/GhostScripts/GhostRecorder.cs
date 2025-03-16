using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    [SerializeField] List<GameObject> ghosts;
    [SerializeField] List<TimeIndicatorHandler> indicatorHandlers;
    [SerializeField] GameObject markerPrefab;

    List<MarkerScript> markerPoints = new List<MarkerScript>();

    bool isHeld = false;
    float[] buttonHoldTimers = new float[4];

    public enum GhostKey
    {
        Ghost1 = KeyCode.Alpha1,
        Ghost2 = KeyCode.Alpha2,
        Ghost3 = KeyCode.Alpha3,
        Ghost4 = KeyCode.Alpha4
    }

    void Awake()
    {
        for (int i = 0; i < ghosts.Count * 2; i++)
        {
            var marker = Instantiate(markerPrefab).GetComponent<MarkerScript>();
            marker.Deactivate();
            markerPoints.Add(marker);
        }
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

        HandleGhostRecording(ghosts[buttonNumber], buttonNumber);
    }

    void OnButtonHeld(int buttonNumber)
    {
        var ghostController = ghosts[buttonNumber].GetComponent<FirstPersonController>();
        if (ghostController == null)
            return;

        ghostController.ImplementedInput.ResetState();

        int startMarker = buttonNumber;
        int endMarker = buttonNumber * 2 + 1;
        markerPoints[startMarker].Deactivate();
        markerPoints[endMarker].Deactivate();

        string color = GetColorString(buttonNumber);
        Debug.Log($"<color={color}>Reset ghost {buttonNumber}</color>");
    }

    void HandleGhostRecording(GameObject ghost, int index)
    {
        var ghostController = ghost.GetComponent<FirstPersonController>();
        if (ghostController == null)
            return;

        var ghostInput = ghostController.ImplementedInput as GhostInput;
        if (ghostInput == null)
            return;

        var playerController = gameObject.GetComponent<FirstPersonController>();
        if (playerController == null)
            return;

        string color = GetColorString(index);
        int startMarker = index;
        int endMarker = index * 2 + 1;

        switch (ghostInput.GhostMovementState)
        {
            // --- START RECORDING ---
            case GhostInput.State.NONE:
                // save starting position and rotation
                ghostInput.SetReplayStartingPosition(transform.position, new Vector2(playerController.pitch, playerController.yaw));
                ghostInput.SetState(GhostInput.State.RECORDING);
                markerPoints[startMarker].Activate(transform.position);
                Debug.Log($"<color={color}>Recording ghost {index}</color>");
                break;
            // --- END RECORDING ---
            case GhostInput.State.RECORDING:
                ghostInput.SetState(GhostInput.State.RECORDED);
                markerPoints[endMarker].Activate(transform.position);
                Debug.Log($"<color={color}>Recorded ghost {index}</color>");
                break;
            // --- REPLAY ---
            case GhostInput.State.RECORDED or GhostInput.State.REPLAY:
                ghostInput.PrepareForReplay();
                ghostInput.SetState(GhostInput.State.REPLAY);
                Debug.Log($"<color={color}>Replay ghost {index}</color>");
                break;
        }
    }

    // for pretty debugging messages c:
    static string GetColorString(int index)
    {
        switch (index)
        {
            case 0:
                return "red";
            case 1:
                return "orange";
            case 2:
                return "yellow";
            case 3:
                return "green";
        }

        return "white";
    }
}
