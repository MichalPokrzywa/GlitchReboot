using UnityEngine;

public class WagonCenterController : MonoBehaviour
{
    [Header("Wheel Settings")]
    [SerializeField] private Transform frontWheel;
    [SerializeField] private Transform rearWheel;
    [SerializeField] private float rotationSmoothing = 5f;
    [SerializeField] private bool lockZRotation = true;

    [Header("Debug")]
    [SerializeField] private bool drawDebugLines = true;

    private void Update()
    {
        if (frontWheel == null || rearWheel == null) return;

        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        // Oblicz œrodek miêdzy ko³ami we wszystkich osiach
        Vector3 midpoint = (frontWheel.position + rearWheel.position) / 2f;
        transform.position = midpoint + new Vector3(0,1.2f,0);
    }

    private void UpdateRotation()
    {
        // Oblicz kierunek jazdy na p³aszczyŸnie X-Z
        Vector3 direction = frontWheel.position - rearWheel.position;
        direction.y = 0; // Ignoruj oœ Y

        if (direction == Vector3.zero) return;

        // Oblicz docelow¹ rotacjê z prawid³owym "góra-dó³"
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        // P³ynna interpolacja rotacji
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothing * Time.deltaTime
        );
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugLines || frontWheel == null || rearWheel == null) return;

        // Rysuj linie pomocnicze
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(frontWheel.position, rearWheel.position);
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}