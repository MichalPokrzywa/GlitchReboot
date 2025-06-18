
using UnityEngine;

public interface InputInterface
{
    public float GetHorizontalInput();
    public float GetVerticalInput();
    public bool IsJumping();
    public bool IsCrouching();
    public bool IsSprinting();
    public float GetMouseX();
    public float GetMouseY();
    public bool OnInteract();
    public bool OnTabletUse();
    public bool OnFire();
    public bool OnEscape();
    public void ResetState();
}
