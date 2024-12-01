using UnityEngine;
using DG.Tweening; // Import biblioteki DoTween

[RequireComponent(typeof(Rigidbody))]
public class PhysicsPlatformMover : MonoBehaviour
{
    // Punkt początkowy platformy
    public Vector3 startPoint;

    // Punkt końcowy platformy
    public Vector3 endPoint;

    // Czas ruchu platformy w sekundach
    public float moveDuration = 2f;

    // Czy platforma ma wrócić do punktu początkowego po ruchu
    public bool returnToStart = true;

    // Komponent Rigidbody
    private Rigidbody rb;

    // Czy platforma jest w trakcie ruchu
    private bool isMoving = false;

    private void Start()
    {
        // Pobierz komponent Rigidbody
        rb = GetComponent<Rigidbody>();

        // Ustaw platformę w punkcie początkowym
        rb.isKinematic = true; // Ustaw Rigidbody jako kinematyczne (ruch kontrolowany przez skrypt)
        transform.position = startPoint;
    }

    // Funkcja wywoływana przez ProximityTrigger
    public void MovePlatform()
    {
        if (isMoving) return; // Ignoruj, jeśli platforma już się porusza

        isMoving = true;

        // Przesuń platformę do punktu końcowego z użyciem DoTween
        rb.DOMove(endPoint, moveDuration).OnComplete(() =>
        {
            if (returnToStart)
            {
                // Wróć do punktu początkowego
                rb.DOMove(startPoint, moveDuration).OnComplete(() =>
                {
                    isMoving = false; // Ruch zakończony
                });
            }
            else
            {
                isMoving = false; // Ruch zakończony
            }
        });
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Obsługuje interakcję z obiektami, które dotykają platformy
        // Możesz dodać dodatkową logikę, jeśli chcesz
    }
}