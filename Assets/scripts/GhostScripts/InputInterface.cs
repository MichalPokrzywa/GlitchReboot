
using UnityEngine;

public interface InputInterface
{
    public float GetHorizontalInput();
    public float GetVerticalInput();
    public bool IsJumping();
    public bool CrouchingStart();
    public bool CrouchingEnd();
    public bool IsSprinting();
    public float GetMouseX();
    public float GetMouseY();
    public void ResetState();
}
