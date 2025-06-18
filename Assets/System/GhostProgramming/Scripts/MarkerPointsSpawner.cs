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
    int selectedMarkerIndex = 0;

    const int markerCount = 9;

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

        for (int i = 0; i < markerPoints.Count; i++)
        {
            var action = InputManager.Instance.GetMarkerAction(i);
            if (action != null && action.triggered)
            {
                OnButtonPressed(i);
                return;
            }
        }

        if (InputManager.Instance.IsNextMarkerPressed())
            CycleMarker(1);
        else if (InputManager.Instance.IsPreviousMarkerPressed())
            CycleMarker(-1);

        if (InputManager.Instance.IsSpawnMarkerPressed())
            OnButtonPressed(selectedMarkerIndex);

    }

    void CycleMarker(int direction)
    {
        selectedMarkerIndex = (selectedMarkerIndex + direction + markerPoints.Count) % markerPoints.Count;
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
