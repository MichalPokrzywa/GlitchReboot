
using UnityEngine;

public interface InputInterface
{
    public float GetMoveHorizontal();
    public float GetMoveVertical();
    public bool IsJumpPressed();
    public bool IsCrouchHeld();
    public bool IsSprintHeld();
    public float GetLookHorizontal();
    public float GetLookVertical();
    public bool IsInteracting();
    public bool IsInteractingWithTablet();
    public bool IsFirePressed();
    public bool IsPausePressed();
    public bool IsZoomHeld();
    public void ResetState();
}
