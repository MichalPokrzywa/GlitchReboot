
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostInput : MonoBehaviour, InputInterface
{
    InputInterface input = new StandardInput();
    float startTime = 0;
    State state = State.NONE;
    Vector3 startingPosition;
    Rigidbody rb;
    RotationController rotationController;

    public State GhostMovementState => state;

    public struct MovementState
    {
        public float timeStamp;
        public float horizontal;
        public float vertical;
        public bool isJumping;
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
        rotationController = GetComponent<RotationController>();
        rb = GetComponent<Rigidbody>();
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
        rotationController.xRotation = replayStartingTransform.rotation.x;
        rotationController.yRotation = replayStartingTransform.rotation.y;
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

        float horizontalInput = input.GetHorizontalInput();
        float verticalInput = input.GetVerticalInput();
        bool isJumping = input.IsJumping();
        float mouseX = input.GetMouseX();
        float mouseY = input.GetMouseY();

        if (recordedMovement.Count > 0 && recordedMovement.Last().horizontal == horizontalInput
            && recordedMovement.Last().vertical == verticalInput && recordedMovement.Last().isJumping == isJumping
            && recordedMovement.Last().mouseX == mouseX && recordedMovement.Last().mouseY == mouseY)
        {
            return;
        }

        MovementState movement = new MovementState
        {
            timeStamp = Time.time - startTime,
            horizontal = horizontalInput,
            vertical = verticalInput,
            isJumping = isJumping,
            mouseX = mouseX,
            mouseY = mouseY
        };

        recordedMovement.Add(movement);
    }

    public void SetState(State state)
    {
        this.state = state;
        startTime = Time.time;

        if (state == State.RECORDING)
        {
            recordedMovement.Clear();
        }
    }

    public float GetHorizontalInput()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.horizontal;
        }

        return 0;
    }

    public float GetVerticalInput()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.vertical;
        }

        return 0;
    }

    public bool IsJumping()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.isJumping;
        }

        return false;
    }

    public float GetMouseX()
    {
        if (state == State.REPLAY)
        {
            return currentMovementState.mouseX;
        }
        return 0;
    }

    public float GetMouseY()
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
}
