using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "ScriptableObjects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    [Serializable]
    public class CameraVariables
    {
        public float FOV = 80;
        public bool enableCameraRotation = true;
        public bool invertCameraRotation = false;
        public float mouseSensitivity = 2;
        public float maxLookAngle = 50;
        public bool enableZoom = true;
        public float zoomFOV = 30;
        public float stepTimeZoom = 10f;
        public KeyCode zoomKey = KeyCode.Mouse1;
    }

    [Serializable]
    public class MovementVariables
    {
        public bool enableWalk = true;
        public float walkSpeed = 5f;
        public bool enableFriction = true;
        [Range(0f, 20f)]
        public float frictionResistance = 5f;
        public bool enableSprint = true;
        public bool unlimitedSprint = true;
        public float sprintSpeed = 7f;
        public float sprintDuration = 5f;
        public float sprintCooldown = 0.5f;
        public float sprintFOV = 90;
        public float stepTimeSprint = 8f;
        public KeyCode sprintKey = KeyCode.LeftShift;
    }

    [Serializable]
    public class JumpVariables
    {
        public bool enableJump = true;
        public float jumpPower = 1f;
        public KeyCode jumpKey = KeyCode.Space;
    }

    [Serializable]
    public class CrouchVariables
    {
        public bool enableCrouch = true;
        public float crouchHeight = 0.75f;
        public float crouchSpeedReduction = 0.5f;
        public KeyCode crouchKey = KeyCode.LeftControl;
    }

    [Serializable]
    public class HeadBobVariables
    {
        public bool enableHeadBob = true;
        public float speed = 10f;
        public Vector3 bobAmount = new Vector3(0f, 0.1f, 0f);
    }

    [SerializeField] CameraVariables _cameraVariables;
    [SerializeField] MovementVariables _movementVariables;
    [SerializeField] JumpVariables _jumpVariables;
    [SerializeField] CrouchVariables _crouchVariables;
    [SerializeField] HeadBobVariables _headBobVariables;

    public CameraVariables cameraVariables => _cameraVariables;
    public MovementVariables movementVariables => _movementVariables;
    public JumpVariables jumpVariables => _jumpVariables;
    public CrouchVariables crouchVariables => _crouchVariables;
    public HeadBobVariables headBobVariables => _headBobVariables;
}