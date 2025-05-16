using UnityEngine;

public class SpiderTestMovement : MonoBehaviour
{
    [Tooltip("Movement speed in units per second along the local X axis.")]
    public float moveSpeed = 1f;
    [Tooltip("Optional input axis name; leave blank for constant movement.")]
    public string inputAxis = "Horizontal";
    [Tooltip("If true, movement is driven by user input; otherwise constant.")]
    public bool useInput = true;
    [Tooltip("Clamp X position between these values (world space).")]
    public Vector2 xLimits = new Vector2(-Mathf.Infinity, Mathf.Infinity);

    void Update()
    {
        float delta = moveSpeed * Time.deltaTime;
        float direction = 1f;
        if (useInput && !string.IsNullOrEmpty(inputAxis))
        {
            direction = Input.GetAxis(inputAxis);
        }

        // Move along local X axis
        Vector3 localOffset = transform.right * direction * delta;
        Vector3 targetPos = transform.position + localOffset;

        // Apply world-space clamp on X
        targetPos.x = Mathf.Clamp(targetPos.x, xLimits.x, xLimits.y);

        transform.position = targetPos;
    }
}
