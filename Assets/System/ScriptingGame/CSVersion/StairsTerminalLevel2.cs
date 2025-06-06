using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class StairsTerminalLevel2 : PuzzleBase
{
    [Header("PuzzleItems")]
    [SerializeField] private List<GameObject> waypoints;
    [SerializeField] private Transform Stairs;
    private Vector3 basicPosition;
    public GameObject waypointParent; // Obiekt zawieraj�cy waypointy

    public void Awake()
    {
        basicPosition = Stairs.transform.position;
    }

    protected override void Start()
    {
        base.Start();
        waypoints = new List<GameObject>(); // Inicjalizujemy list�, na wszelki wypadek
        if (waypointParent == null)
        {
            // Debug.LogError("Stairs: waypointParent nie zosta� przypisany w Inspektorze!");
            return; // Przerywamy dzia�anie, je�li nie ma rodzica
        }


        foreach (Transform child in waypointParent.transform)
        {
            if (child == null)
            {
                continue; // Przechodzimy do nast�pnej iteracji
            }
            waypoints.Add(child.gameObject);
        }

    }


    public override void DoTerminalCode()
    {
        int platformx = GetVariableValue<int>("x");
        int platformy = GetVariableValue<int>("y");
        int platformz = GetVariableValue<int>("z");
        int platformBaseHight = GetVariableValue<int>("Base_Height");

        if (waypoints == null || waypoints.Count == 0)
        {
            return; // Przerywamy dzia�anie, je�li nie ma waypoint�w
        }

        Vector3 targetPosition = new Vector3(
        basicPosition.x,
        basicPosition.y + platformBaseHight,
        basicPosition.z
        );

        StartCoroutine(SmoothMoveStairs(targetPosition, 1.0f));
        int iteration = 0;
        foreach (GameObject waypoint in waypoints)
        {
            Vector3 newPosition = Stairs.position + new Vector3(
                platformx * iteration,
                platformy * iteration,
                platformz * iteration
            );
            waypoint.transform.position = newPosition;
            // Debug.Log("Stairs: Ustawiono pozycj� waypointa " + waypoint.name + " na: " + newPosition);
            iteration++;
        }
    }
    // Coroutine, kt�ra �przesuwa� obiekt Stairs z pozycji startowej do targetPosition w czasie duration.
    private IEnumerator SmoothMoveStairs(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = Stairs.position;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Vector3.Lerp interpoluje liniowo mi�dzy start a target.
            Stairs.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;  // Czekamy do nast�pnej klatki
        }

        // Upewniamy si�, �e obiekt trafia dok�adnie do docelowej pozycji.
        Stairs.position = targetPosition;
    }

}