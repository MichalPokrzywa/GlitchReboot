using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject waypointParent; // Obiekt zawierający waypointy
    public float moveSpeed = 5f; // Prędkość poruszania się
    public float moveInterval = 1f; // Czas pomiędzy ruchem do następnego waypointa
    public float stoppingDistance = 0.05f; // Dystans, przy którym uznajemy, że dotarliśmy do celu
    public CustomTrigger joiningPlatformTrigger;
    public CustomTrigger exitingPlatformTrigger;
    
    private List<Transform> waypoints; // Lista waypointów
    private int index = 0; // Indeks aktualnego waypointa
    private float timer;
    private bool objectInMotion = false;
    
    void Awake()
    {
        joiningPlatformTrigger.EnteredTrigger.AddListener(OnJoiningPlatformTrigger);
        exitingPlatformTrigger.ExitedTrigger.AddListener(OnExitingPlatformTrigger);
    }

    void Start()
    {
        // Pobranie wszystkich waypointów z obiektu rodzica
        waypoints = new List<Transform>();
        foreach (Transform child in waypointParent.transform)
        {
            waypoints.Add(child);
        }
        
        timer = moveInterval; // Ustawienie początkowego timera
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0 && !objectInMotion)
        {
            objectInMotion = true; // Rozpoczęcie ruchu
        }

        if (objectInMotion && waypoints.Count > 0)
        {
            // Obliczenie kierunku i przesunięcie obiektu w stronę waypointa
            Vector3 targetPosition = waypoints[index].position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Sprawdzenie, czy dotarliśmy do waypointa
            if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
            {
                // Przejście do następnego waypointa
                index = (index + 1) % waypoints.Count;
                objectInMotion = false; // Wstrzymanie ruchu
                timer = moveInterval; // Restart timera
            }
        }
    }
    
    private void OnJoiningPlatformTrigger(Collider collider)
    {
        collider.transform.SetParent(transform);
    }

    private void OnExitingPlatformTrigger(Collider collider)
    {
        // Debug.Log($"Obiekt {collider.gameObject.name} wszedł w trigger {gameObject.name}, status dziecka:{IsMyDirectChild(collider.gameObject)}");
        if (IsMyDirectChild(collider.gameObject)) {collider.transform.SetParent(null);}
    }

    private void Reset()
    {
        EnsureTriggerExists(ref joiningPlatformTrigger, "JoiningPlatformTrigger");
        EnsureTriggerExists(ref exitingPlatformTrigger, "ExitingPlatformTrigger");
    }
    
    private bool IsMyDirectChild(GameObject obj)
    {
        return obj != null && obj.transform.parent == transform;
    }
    
    private void EnsureTriggerExists(ref CustomTrigger trigger, string triggerName)
    {
        if (trigger == null)
        {
            Transform existingTrigger = transform.Find(triggerName);
            
            if (existingTrigger == null)
            {
                GameObject triggerObject = new GameObject(triggerName);
                triggerObject.transform.SetParent(transform);
                triggerObject.transform.localPosition = Vector3.zero;
                triggerObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                Collider collider = triggerObject.AddComponent<BoxCollider>(); // Domyślnie BoxCollider
                collider.isTrigger = true;

                trigger = triggerObject.AddComponent<CustomTrigger>();

                Debug.Log($"Stworzono brakujący trigger: {triggerName}");
            }
            else
            {
                trigger = existingTrigger.GetComponent<CustomTrigger>();
            }
        }
    }
}
