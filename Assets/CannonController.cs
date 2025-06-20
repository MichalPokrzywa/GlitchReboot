using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Obiekty")]
    public Transform firePoint;

    [Header("Ustawienia wystrzału")]
    public float initialSpeed = 20f;
    public float launchAngleDegrees = 0.0f;

    private void Update()
    {
        Debug.DrawRay(firePoint.position, GetLaunchDirection() * 5f, Color.red, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            Launch(rb);
        }
    }

    private Vector3 GetLaunchDirection()
    {
        Quaternion rotation = Quaternion.AngleAxis(launchAngleDegrees, firePoint.right);
        return rotation * firePoint.forward;
    }
    private IEnumerator OrientDuringFlight(Rigidbody rb, FirstPersonController playerController = null)
    {
        if (rb == null)
        {
            if (playerController != null)
            {
                playerController.enabled = true; // Włącz kontroler, jeśli Rigidbody zniknęło
                Debug.Log("OrientDuringFlight: Rigidbody is null, FirstPersonController gracza włączony.");
            }
            yield break;
        }

        // Poczekaj, aż prędkość spadnie do progu
        while (rb != null && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            if (rb == null) // Sprawdzenie na wypadek zniszczenia obiektu w trakcie lotu
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
            yield return null; // Poczekaj do następnej klatki
        }

        // Gdy pętla się zakończy (obiekt prawie się zatrzymał)
        if (playerController != null)
        {
            playerController.enabled = true; // Włącz kontroler gracza
            Debug.Log("FirstPersonController gracza został ponownie włączony po lądowaniu.");
        }

        Debug.Log($"{rb?.name ?? "Obiekt"} zakończył orientację podczas lotu.");
    }
    private void Launch(Rigidbody rb)
    {
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
        // Przenieś do lufy
        rb.transform.position = firePoint.position;
        rb.transform.rotation = Quaternion.LookRotation(GetLaunchDirection());
        rb.isKinematic = false;

        // Nadaj prędkość początkową
        Vector3 velocity = GetLaunchDirection().normalized * initialSpeed;
        rb.linearVelocity = velocity;
        StartCoroutine(OrientDuringFlight(rb, playerController));
        Debug.Log($"{rb.name} został wystrzelony z armaty.");
    }
}