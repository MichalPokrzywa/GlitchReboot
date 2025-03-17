using System;
using UnityEngine;

public class ActivableMoveObject : MonoBehaviour, IActivatable
{
    public Vector3 moveDirection = new Vector3(1, 0, 0);
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    private Vector3 targetPosition;
    private Vector3 targetPosition2;
    private bool shouldMove = false;
    private Vector3 basicPosition;

    private void Start()
    {
        basicPosition = transform.position;
    }

    public void Activate()
    {
        targetPosition2 = transform.position + moveDirection.normalized * moveDistance;
        targetPosition = targetPosition2;
        shouldMove = true;
    }

    public void Deactivate()
    {
        targetPosition = basicPosition;
        shouldMove = true;
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
