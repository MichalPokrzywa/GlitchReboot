using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : ParentOnTrigger
{
    public GameObject waypointParent; // Obiekt zawierający waypointy
    public float moveSpeed = 5f; // Prędkość poruszania się
    public float moveInterval = 1f; // Czas pomiędzy ruchem do następnego waypointa
    public float stoppingDistance = 0.05f; // Dystans, przy którym uznajemy, że dotarliśmy do celu
    
    private List<Transform> waypoints; // Lista waypointów
    private int index = 0; // Indeks aktualnego waypointa
    private float timer;
    private bool objectInMotion = false;
    
    
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
    
    
    
    
}
