using UnityEngine;

public class WaypointGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.magenta;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
