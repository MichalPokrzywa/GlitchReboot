
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class GhostInput : MonoBehaviour, InputInterface
{
    InputInterface input = new StandardInput();
    float startTime = 0;
    State state = State.NONE;
    Vector3 startingPosition;
    Rigidbody rb;
    FirstPersonController controller;

    public State GhostMovementState => state;

    public struct MovementState
    {
        public float timeStamp;
        public float horizontal;
        public float vertical;
        public bool isJumping;
        public bool isCrouching;
        public bool isSprinting;
        public float mouseX;
        public float mouseY;
    }

    public struct ReplayStartingTransform
    {
        public Vector3 position;
        public Vector2 rotation;
    }

    public enum State
    {
        NONE = 0,
        RECORDING = 1,
        RECORDED = 2,
        REPLAY = 3
    }

    List<MovementState> recordedMovement = new List<MovementState>();
    List<MovementState> remainingMovement = new List<MovementState>();
    MovementState currentMovementState = new MovementState();
    ReplayStartingTransform replayStartingTransform = new ReplayStartingTransform();

    void Awake()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<FirstPersonController>();
    }

    void FixedUpdate()
    {
        if (state == State.RECORDING)
            UpdateRecording();

        if (state == State.REPLAY)
            UpdateReplaying();
    }

    public void PrepareForReplay()
    {
        remainingMovement.Clear();
        remainingMovement.AddRange(recordedMovement);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = Vector3.zero;
        rb.rotation = Quaternion.identity;

        transform.position = replayStartingTransform.position;
        controller.Rotate(replayStartingTransform.rotation.x, replayStartingTransform.rotation.y);
    }

    public void SetReplayStartingPosition(Vector3 position, Vector2 rotation)
    {
        replayStartingTransform.position = position;
        replayStartingTransform.rotation = rotation;
    }

    void UpdateReplaying()
    {
        if (remainingMovement.Count == 0)
        {
            return;
        }

        if (Time.time - startTime >= remainingMovement.First().timeStamp)
        {
            currentMovementState = remainingMovement.First();
            remainingMovement.RemoveAt(0);
        }
    }

    void UpdateRecording()
    {
        if (state != State.RECORDING)
        {
            return;
        }

        float horizontalInput = input.GetMoveHorizontal();
        float verticalInput = input.GetMoveVertical();
        bool isJumping = input.IsJumpPressed();
        bool isCrouching = input.IsCrouchHeld();
        bool isSprinting = input.IsSprintHeld();
        float mouseX = input.GetLookHorizontal();
        float mouseY = input.GetLookVertical();

        bool isCurrentInputRepeated = recordedMovement.Count > 0
            && recordedMovement.Last().horizontal == horizontalInput
            && recordedMovement.Last().vertical == verticalInput
            && recordedMovement.Last().isJumping == isJumping
            && recordedMovement.Last().isCrouching == isCrouching
            && recordedMovement.Last().isSprinting == isSprinting
            && recordedMovement.Last().mouseX == mouseX
            && recordedMovement.Last().mouseY == mouseY;

        if (isCurrentInputRepeated)
        {
            return;
        }

        MovementState movement = new MovementState
        {
            timeStamp = Time.time - startTime,
            horizontal = horizontalInput,
            vertical = verticalInput,
            isJumping = isJumping,
            isCrouching = isCrouching,
            isSprinting = isSprinting,
            mouseX = mouseX,
            mouseY = mouseY
        };

        recordedMovement.Add(movement);
    }

    public void SetState(State state)
    {
        this.state = state;
        if (state == State.RECORDING)
        {
            recordedMovement.Clear();
        }

        // Add a final movement command to ensure the ghost stops properly
        if (state == State.RECORDED)
        {
            MovementState movement = new MovementState
            {
                timeStamp = Time.time - startTime,
                horizontal = 0,
                vertical = 0,
                isJumping = false,
                isCrouching = false,
                isSprinting = false,
                mouseX = 0,
                mouseY = 0
            };

            recordedMovement.Add(movement);
        }

        startTime = Time.time;
    }

    public float GetMoveHorizontal()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.horizontal;
        }

        return 0;
    }

    public float GetMoveVertical()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.vertical;
        }

        return 0;
    }

    public bool IsJumpPressed()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.isJumping;
        }

        return false;
    }

    public bool IsCrouchHeld()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.isCrouching;
        }

        return false;
    }

    public bool IsSprintHeld()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.isSprinting;
        }

        return false;
    }

    public float GetLookHorizontal()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.mouseX;
        }
        return 0;
    }

    public float GetLookVertical()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.mouseY;
        }
        return 0;
    }

    public void ResetState()
    {
        recordedMovement.Clear();
        remainingMovement.Clear();
        currentMovementState = new MovementState();
        transform.position = startingPosition;
        state = State.NONE;
    }

    public bool IsInteracting()
    {
        throw new System.NotImplementedException();
    }

    public bool IsInteractingWithTablet()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFirePressed()
    {
        throw new System.NotImplementedException();
    }

    public bool IsPausePressed()
    {
        throw new System.NotImplementedException();
    }

    public bool IsZoomHeld()
    {
        throw new System.NotImplementedException();
    }
}
