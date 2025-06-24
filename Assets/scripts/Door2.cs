using UnityEngine;

public class Door2 : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    private Vector3 baseLeftDoorPosition;
    private Vector3 baseRightDoorPosition;
    private Vector3 leftTargetPosition;
    private Vector3 rightTargetPosition;

    private bool isOpening = false;
    private bool isClosing = false;

    private void Start()
    {
        baseLeftDoorPosition = leftDoor.transform.position;
        baseRightDoorPosition = rightDoor.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpening = true;
            isClosing = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpening = false;
            isClosing = true;
        }
    }

    private void Update()
    {
        if (isOpening)
        {
            // Ustaw pozycje docelowe
            leftTargetPosition = baseLeftDoorPosition + Vector3.left * 1.5f;
            rightTargetPosition = baseRightDoorPosition + Vector3.right * 1.5f;

            // Przesuwanie drzwi
            leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, leftTargetPosition, Time.deltaTime);
            rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, rightTargetPosition, Time.deltaTime);
        }
        else if (isClosing)
        {
            leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, baseLeftDoorPosition, Time.deltaTime);
            rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, baseRightDoorPosition, Time.deltaTime);
        }
    }
}