using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Transform target;       // The object to orbit around
    public float distance = 5.0f;  // Distance from the target
    public float orbitSpeed = 20.0f; // Degrees per second
    
    private float currentAngle = 0f;
    
    void Update()
    {
        if (target == null) return;
    
        // Increment the angle based on time and speed
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;
    
        // Convert angle to radians
        float angleRad = currentAngle * Mathf.Deg2Rad;
    
        // Calculate new position
        Vector3 offset = new Vector3(Mathf.Sin(angleRad),0.5f, Mathf.Cos(angleRad)) * distance;
        transform.position = target.position + offset;
    
        // Look at the target
        transform.LookAt(target);
    }
}
