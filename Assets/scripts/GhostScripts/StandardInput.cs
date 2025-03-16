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
        return Input.GetKey(KeyCode.Space);
    }

    public bool IsCrouching()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }

    public bool IsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public float GetMouseX()
    {
        return Input.GetAxisRaw("Mouse X");
    }

    public float GetMouseY()
    {
        return Input.GetAxisRaw("Mouse Y");
    }

    public void ResetState()
    {
        return;
    }
}
