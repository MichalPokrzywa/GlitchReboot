using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // !!! DODAJ TĘ LINIĘ !!!

public class CannonController : MonoBehaviour
{
    [Header("Obiekty")]
    public Transform firePoint;

    [Header("Ustawienia wystrzału")]
    public float initialSpeed = 20f;
    public float launchAngleDegrees = 0.0f;

    [Header("Ustawienia animacji przed wystrzałem")]
    public float preLaunchAnimationDuration = 0.5f; // Czas trwania animacji do firePoint
    public float preLaunchJumpPower = 1.0f; // Wysokość "podskoku" (dla DOJump)
    public int preLaunchNumJumps = 1; // Liczba "podskoków" (dla DOJump)

    private void Update()
    {
        Debug.DrawRay(firePoint.position, GetLaunchDirection() * 5f, Color.red, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // FirstPersonController gracza będzie wyłączony i włączony w Launch/OrientDuringFlight
        }

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            var interactor = rb.GetComponent<Interactor>();
            if (interactor != null && interactor.IsHoldingObject())
                return;

            Launch(rb);
        }
    }

    private Vector3 GetLaunchDirection()
    {
        // Upewnij się, że firePoint.right jest odpowiednią osią obrotu
        Quaternion rotation = Quaternion.AngleAxis(launchAngleDegrees, firePoint.right);
        return rotation * firePoint.forward;
    }

    private IEnumerator OrientDuringFlight(Rigidbody rb, FirstPersonController playerController = null)
    {
        if (rb == null)
        {
            if (playerController != null)
            {
                playerController.enabled = true;
                Debug.Log("OrientDuringFlight: Rigidbody jest null, FirstPersonController gracza włączony.");
            }
            yield break;
        }

        // Poczekaj, aż prędkość spadnie do progu
        while (rb != null && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            if (rb == null)
            {
                if (playerController != null)
                {
                    playerController.enabled = true;
                    Debug.Log("OrientDuringFlight: Rigidbody stało się null, FirstPersonController gracza włączony.");
                }
                yield break;
            }

            Vector3 currentVelocity = rb.linearVelocity;
            if (currentVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
                rb.MoveRotation(targetRotation);
            }
            yield return null;
        }

        // Gdy pętla się zakończy (obiekt prawie się zatrzymał)
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("FirstPersonController gracza został ponownie włączony po lądowaniu.");
        }

        Debug.Log($"{rb?.name ?? "Obiekt"} zakończył orientację podczas lotu.");
    }

    private void Launch(Rigidbody rb)
    {
        // Sprawdź, czy wystrzelimy gracza
        FirstPersonController playerController = null;
        if (rb.CompareTag("Player")) // Zakładamy, że gracz ma tag "Player"
        {
            playerController = rb.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                playerController.enabled = false; // Wyłącz kontroler gracza przed wystrzałem
                Debug.Log("FirstPersonController gracza został wyłączony.");
            }
        }

        // Upewnij się, że Rigidbody jest wyłączone kinematic na czas animacji,
        // aby fizyka nie przeszkadzała w animacji DOTween.
        // Jeśli obiekt był już kinematic, to tutaj to resetujemy
        bool wasKinematic = rb.isKinematic;
        rb.isKinematic = true;

        // Animaacja obiektu do firePoint przed wystrzałem
        rb.transform.DOJump(firePoint.position, preLaunchJumpPower, preLaunchNumJumps, preLaunchAnimationDuration)
            .SetEase(Ease.OutQuad) // Płynne przyspieszenie i zwolnienie
            .OnComplete(() =>
            {
                // To wywoła się, gdy animacja DOTween się zakończy
                rb.isKinematic = wasKinematic; // Przywróć poprzedni stan kinematic
                if (!wasKinematic) // Jeśli nie było kinematic, ustaw na false, aby fizyka działała
                {
                    rb.isKinematic = false;
                }

                // Ustaw rotację obiektu w kierunku wystrzału
                rb.transform.rotation = Quaternion.LookRotation(GetLaunchDirection());

                // Nadaj prędkość początkową
                Vector3 velocity = GetLaunchDirection().normalized * initialSpeed;
                rb.linearVelocity = velocity; // Używamy linearVelocity dla DOTween
                rb.angularVelocity = Vector3.zero; // Opcjonalnie zresetuj rotację, jeśli nie chcesz, żeby obiekt się kręcił

                // Rozpocznij korutynę do orientowania obiektu w locie
                StartCoroutine(OrientDuringFlight(rb, playerController));

                Debug.Log($"{rb.name} został wystrzelony z armaty po animacji.");
            });
    }
}