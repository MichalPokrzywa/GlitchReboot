using UnityEngine;

public class StandardInput : InputInterface
{
    public float GetMoveHorizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetMoveVertical()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public bool IsJumpPressed()
    {
        // Space or 'A' on Xbox controller
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0);
    }

    public bool IsCrouchHeld()
    {
        // LCtrl or 'B' on Xbox controller
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.JoystickButton1);
    }

    public bool IsSprintHeld()
    {
        // LShift or LT
        return Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("Fire3") > 0.1f;
    }

    public float GetLookHorizontal()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float stickX = Input.GetAxisRaw("Joystick X");
        return Mathf.Abs(mouseX) > 0.01f ? mouseX : stickX;
    }

    public float GetLookVertical()
    {
        float mouseY = Input.GetAxisRaw("Mouse Y");
        float stickY = Input.GetAxisRaw("Joystick Y");
        return Mathf.Abs(mouseY) > 0.01f ? mouseY : stickY;
    }

    public bool IsInteracting()
    {
        // E or 'X' on Xbox controller
        return Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2);
    }

    public bool IsInteractingWithTablet()
    {
        // Q or 'Y' on Xbox controller
        return Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3);
    }

    public bool IsFirePressed()
    {
        // LMB or 'LB' on Xbox controller
        return Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton4);
    }

    public bool IsPausePressed()
    {
        // ESC or 'Start' on Xbox controller
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7);
    }

    public bool IsZoomHeld()
    {
        // RMB or 'RB' on Xbox controller
        return Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.JoystickButton5);
    }

    public void ResetState()
    {
        return;
    }
}
