using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public Transform cameraPosition;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
