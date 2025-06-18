using UnityEngine;

public class StandardInput : InputInterface
{
    public float GetHorizontalInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetVerticalInput()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public bool IsJumping()
    {
        // Space or 'A' on Xbox controller
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0);
    }

    public bool IsCrouching()
    {
        // LCtrl or 'B' on Xbox controller
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.JoystickButton1);
    }

    public bool IsSprinting()
    {
        // LShift or LT
        return Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("Fire3") > 0.1f;
    }

    public float GetMouseX()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float stickX = Input.GetAxisRaw("Joystick X");
        return Mathf.Abs(mouseX) > 0.01f ? mouseX : stickX;
    }

    public float GetMouseY()
    {
        float mouseY = Input.GetAxisRaw("Mouse Y");
        float stickY = Input.GetAxisRaw("Joystick Y");
        return Mathf.Abs(mouseY) > 0.01f ? mouseY : stickY;
    }

    public bool OnInteract()
    {
        // E or 'X' on Xbox controller
        return Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2);
    }

    public bool OnTabletUse()
    {
        // Q or 'LB'
        return Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton4);
    }

    public bool OnFire()
    {
        // Left mouse button or 'Y' on Xbox controller
        return Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton3);
    }

    public bool OnEscape()
    {
        // ESC or 'Start' on Xbox controller
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7);
    }

    public void ResetState()
    {
        return;
    }
}
