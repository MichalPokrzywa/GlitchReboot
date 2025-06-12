using System;
using System.Collections.Generic;
using UnityEngine;

public class MarkerPointsSpawner : MonoBehaviour
{
    [SerializeField] GameObject markerPrefab;
    [SerializeField] Transform target;
    [SerializeField] bool cursorRaycast = true;
    [SerializeField] float cursorOffsetDistance = 0.5f;

    public List<MarkerScript> markers => markerPoints;
    public bool active = true;

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
        Color[] markerColors = {
            Color.white, Color.red, Color.green,
            Color.blue, Color.magenta, Color.yellow, Color.cyan,
            Color.black, new Color(0.74f, 0.17f, 0f)
        };

        for (int i = 0; i < markerCount; i++)
        {
            var marker = Instantiate(markerPrefab).GetComponent<MarkerScript>();
            marker.SetTarget(target);
            int number = i + 1;
            marker.SetText(number.ToString());
            float intensityValue = 3f;
            Color color = markerColors[i % markerColors.Length] * intensityValue;
            marker.SetColor(color);
            markerPoints.Add(marker);
        }
    }

    void Update()
    {
        if (!active)
            return;

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
        {
            marker.Deactivate();
            return;
        }

        if (!cursorRaycast)
        {
            marker.Activate(target.position);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                Vector3 offset = hit.normal * cursorOffsetDistance;
                marker.Activate(hit.point + offset);
            }
            else
            {
                marker.Activate(target.position);
            }
        }
        else
        {
            marker.Activate(target.position);
        }
    }
}
