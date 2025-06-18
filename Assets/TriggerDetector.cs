using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] private Transform train;

    void OnTriggerEnter(Collider other)
    {
        train.GetChild(0).GetComponent<TrainController>().drive = false;
        gameObject.SetActive(false);
    }
}
