using System;
using System.Collections.Generic;
using UnityEngine;

public class MarkerPointsSpawner : MonoBehaviour
{
    [SerializeField] GameObject markerPrefab;
    [SerializeField] Transform target;

    public List<MarkerScript> markers => markerPoints;

    List<MarkerScript> markerPoints = new List<MarkerScript>();

    const int markerCount = 9;

    public enum MarkerPointKey
    {
        Marker1 = KeyCode.Alpha1,
        Marker2 = KeyCode.Alpha2,
        Marker3 = KeyCode.Alpha3,
        Marker4 = KeyCode.Alpha4,
        Marker5 = KeyCode.Alpha5,
        Marker6 = KeyCode.Alpha6,
        Marker7 = KeyCode.Alpha7,
        Marker8 = KeyCode.Alpha8,
        Marker9 = KeyCode.Alpha9,
    }

    void Awake()
    {
        for (int i = 0; i < markerCount; i++)
        {
            var marker = Instantiate(markerPrefab).GetComponent<MarkerScript>();
            marker.Deactivate();
            marker.SetTarget(target);
            int number = i + 1;
            marker.SetText(number.ToString());
            markerPoints.Add(marker);
        }
    }

    void Update()
    {
        int index = 0;
        foreach (MarkerPointKey key in Enum.GetValues(typeof(MarkerPointKey)))
        {
            CheckButtonPress(key, index);
            index++;
        }
    }

    void CheckButtonPress(MarkerPointKey markerKey, int index)
    {
        if (Input.GetKeyUp((KeyCode)markerKey))
            OnButtonPressed(index);
    }

    void OnButtonPressed(int buttonNumber)
    {
        if (buttonNumber < 0 || buttonNumber >= markerPoints.Count)
            return;

        var marker = markerPoints[buttonNumber];

        if (marker.isActive)
            marker.Deactivate();
        else
            marker.Activate(target.position);
    }
}
