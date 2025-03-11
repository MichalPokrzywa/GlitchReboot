
using UnityEngine;

public interface InputInterface
{
    public float GetHorizontalInput();
    public float GetVerticalInput();
    public bool IsJumping();
    public bool IsCrouching();
    public  bool IsSprinting();
    public float GetMouseX();
    public float GetMouseY();
    public void ResetState();
}
