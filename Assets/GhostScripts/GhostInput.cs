
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostInput : MonoBehaviour, InputInterface
{
    InputInterface input = new StandardInput();
    float startTime = 0;
    State state = State.NONE;

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

    public enum State
    {
        NONE = 0,
        RECORDING = 1,
        RECORDED = 2,
        REPLAY = 3
    }

    List<MovementState> recordedMovement = new List<MovementState>();
    MovementState currentMovementState = new MovementState();

    void FixedUpdate()
    {
        if (state == State.RECORDING)
            UpdateRecording();

        if (state == State.REPLAY)
            UpdateReplaying();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2);
    }

    void UpdateReplaying()
    {
        if (recordedMovement.Count == 0)
        {
            return;
        }

        if (Time.time - startTime >= recordedMovement.First().timeStamp)
        {
            currentMovementState = recordedMovement.First();
            recordedMovement.RemoveAt(0);
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
}
