using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsPlatformMover : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("Koniec ruchu platformy")]
    public GameObject end;

    [Tooltip("Czas trwania ruchu w sekundach")]
    public float moveDuration = 2f;

    [Tooltip("Czy platforma wraca do pozycji początkowej po braku aktywności")]
    public bool returnToStart = true;

    [Tooltip("Opóźnienie przed powrotem na start po braku wywołań (sekundy)")]
    public float returnDelay = 2f;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Rigidbody rb;
    private bool isMoving = false;
    private float returnTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        startPoint = transform.position;
        endPoint = end.transform.position;
    }

    private void Update()
    {
        // Jeśli powrót do pozycji początkowej jest włączony, uruchamiamy odliczanie
        if (returnToStart && !isMoving)
        {
            returnTimer -= Time.deltaTime;

            if (returnTimer <= 0f && Vector3.Distance(transform.position, startPoint) > 0.1f)
            {
                MoveToPosition(startPoint, null);
            }
        }
    }

    public void MovePlatform()
    {
        returnTimer = returnDelay; // Resetuj licznik powrotu

        if (isMoving) return; // Ignoruj, jeśli platforma jest w ruchu

        isMoving = true;

        MoveToPosition(endPoint, () =>
        {
            isMoving = false; // Platforma osiągnęła punkt końcowy
        });
    }

    private void MoveToPosition(Vector3 targetPosition, TweenCallback onComplete)
    {
        rb.DOMove(targetPosition, moveDuration).OnComplete(onComplete);
    }
}
