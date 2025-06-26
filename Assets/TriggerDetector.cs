using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] private Transform train;

    void OnTriggerEnter(Collider other)
    {
        train.GetComponent<TrainController>().drive = false;
        gameObject.SetActive(false);
    }
}
