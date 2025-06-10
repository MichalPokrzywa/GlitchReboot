using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : ParentOnTrigger, IActivatable
{
    public GameObject waypointParent; // Obiekt zawierający waypointy
    public float moveSpeed = 5f; // Prędkość poruszania się
    public float moveInterval = 1f; // Czas pomiędzy ruchem do następnego waypointa
    public float stoppingDistance = 0.1f; // Dystans, przy którym uznajemy, że dotarliśmy do celu
    public bool startMoving = false; // Flaga do rozpoczęcia ruchu
    public float zOscillationSpeed = 0.0f;
    public float maxZOscilation = 0.0f;

    public List<Transform> waypoints; // Lista waypointów
    private int index = 0; // Indeks aktualnego waypointa
    private float timer;
    private bool objectInMotion = false;
    private Vector3 basePosition;
    

    public void Activate()
    {
        startMoving = false;
    }

    public void Deactivate()
    {
        startMoving = true; // Umożliwienie ruchu
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

        basePosition = transform.position;
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if ((timer <= 0 && !objectInMotion) && startMoving)
        {
            objectInMotion = true; // Rozpoczęcie ruchu
        }

        if (objectInMotion && waypoints.Count > 0)
        {
            MoveWithZOscilation(); // Wywołanie metody poruszania się
        }
        else
        {
            ZOscilate();
        }        
    }

    private void MoveWithZOscilation()
    {
        // Obliczenie kierunku i przesunięcie obiektu w stronę waypointa
        Vector3 targetPosition = waypoints[index].position;

        basePosition = Vector3.MoveTowards(basePosition, targetPosition, moveSpeed * Time.deltaTime);

        float zOffset = (float)Math.Sin(Time.time * zOscillationSpeed) * maxZOscilation;
        
        transform.position = new Vector3 (basePosition.x, basePosition.y, basePosition.z + zOffset);
        
        
        // Sprawdzenie, czy dotarliśmy do waypointa
        if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            // Przejście do następnego waypointa
            index = (index + 1) % waypoints.Count;
            objectInMotion = false; // Wstrzymanie ruchu
            timer = moveInterval; // Restart timera
        }

    }

    private void ZOscilate()
    {
        float zOffset = Mathf.Sin(Time.time * zOscillationSpeed) * maxZOscilation;
        transform.position = new Vector3(basePosition.x, basePosition.y, basePosition.z + zOffset);
    }
}
