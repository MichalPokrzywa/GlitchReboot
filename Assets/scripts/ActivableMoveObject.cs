using UnityEngine;

public class ActivableMoveObject : MonoBehaviour, IActivatable
{
    public Vector3 moveDirection = new Vector3(1, 0, 0);
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    private Vector3 targetPosition;
    private bool shouldMove = false;

    public void Activate()
    {
        if (!shouldMove)
        {
            targetPosition = transform.position + moveDirection.normalized * moveDistance;
            shouldMove = true;
        }
    }

    void Update()
    {
        if (shouldMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                shouldMove = false;
            }
        }
    }
}
