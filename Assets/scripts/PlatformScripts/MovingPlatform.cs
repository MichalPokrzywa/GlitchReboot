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
    public float zOscilation = 0.0f;

    public List<Transform> waypoints; // Lista waypointów
    private int index = 0; // Indeks aktualnego waypointa
    private float timer;
    private bool objectInMotion = false;
    public bool ZOscilationDirection = true;
    public float zStartingPosition;
    public float zOscilationPosition;
    public float zOscilationNegativePosition;
    private float maxZOscilation = 10.0f;

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
        if (zOscilation > 0.1f)
        {
            zStartingPosition = gameObject.transform.position.z;
            zOscilationPosition = zStartingPosition + zOscilation;
            zOscilationNegativePosition = zStartingPosition - zOscilation;
        }
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
            Move(); // Wywołanie metody poruszania się
        }
        
        if (zOscilation > 0.1f)
            ZOscilate();
        
    }

    private void Move()
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

    private void ZOscilate()
    {
        
        if (ZOscilationDirection)
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(transform.position.x, transform.position.y, transform.position.z + maxZOscilation), 
                moveSpeed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, transform.position.y, transform.position.z - maxZOscilation),
                moveSpeed * Time.deltaTime);
        if (transform.position.z >= zOscilationPosition)
            ZOscilationDirection = false;
        if (transform.position.z <= zOscilationNegativePosition)
            ZOscilationDirection = true;
        Debug.Log("Wywoluje sie");
        Debug.Log(transform.position.z + maxZOscilation);
        Debug.Log(transform.position.z);
        Debug.Log(moveSpeed);

    }
}
