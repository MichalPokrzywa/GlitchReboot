using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        transform.GetChild(0).GetComponent<TrainController>().drive = false;
        Debug.Log("kurwa szmata pierdolona");
    }
}
