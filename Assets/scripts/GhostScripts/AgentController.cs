using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    [SerializeField] Transform holdPoint;

    // -------------------------------------------------
    [SerializeField] GameObject objectTEST;

    // for cubes: 2f, platforms: 0.1f
    public float interactionDistance = 2f;

    Action onTargetReached;

    NavMeshAgent agent;
    GameObject searchingObject;
    Coroutine pathCoroutine;
    GameObject lastTarget = null;
    Vector3? lastTargetUnreachablePosition = null;

    GameObject pickedUpObject;

    float distance;
    bool hasReachedTarget = false;
    bool targetUnreachable = false;

    const float pathCalculationInterval = 0.5f;
    const float hysteresisBuffer = 0.2f;

    // ----------------------------------------------------------------------------------------

    public void MoveToTEST()
    {
        Debug.Log("Moving to: " + objectTEST.name);
        MoveTo(objectTEST);
    }

    public void StopFollowingTEST()
    {
        Debug.Log("Stopped Following");
        StopFollowing();
    }

    public void PickUpTEST()
    {
        Debug.Log("I'm going to pick up an object...");
        var pickUpComponent = objectTEST.GetComponent<PickUpObjectInteraction>();
        PickUp(pickUpComponent);
    }

    public void DropTEST()
    {
        Debug.Log("Dropping...");
        Drop();
    }

    // ----------------------------------------------------------------------------------------

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (searchingObject == null)
        {
            return;
        }

        // Check if we should consider path recalculation
        if (targetUnreachable)
        {
            if (!HasUnreachableTargetMoved())
            {
                return;
            }
        }

        distance = Vector3.Distance(agent.transform.position, searchingObject.transform.position);

        // If target was not reached yet, but we are close enough - stop path finding
        if (!hasReachedTarget && distance < interactionDistance)
        {
            agent.isStopped = true;
            hasReachedTarget = true;
            ResetCoroutine();
            onTargetReached?.Invoke();
            onTargetReached = null;
        }
        // if target was reached already, but it moved away - start path finding again
        else if (distance > interactionDistance + hysteresisBuffer)
        {
            agent.isStopped = false;
            hasReachedTarget = false;

            if (pathCoroutine == null)
            {
                pathCoroutine ??= StartCoroutine(MoveToTargetCoroutine());
                Debug.Log("Coroutine started");
            }
        }
    }

    public void MoveTo(GameObject newSearchingObject)
    {
        if (newSearchingObject == null)
        {
            Debug.LogWarning("New searching object is null.");
            return;
        }

        searchingObject = newSearchingObject;
        lastTarget = newSearchingObject;
        targetUnreachable = false;
        hasReachedTarget = false;
    }

    public void StopFollowing()
    {
        if (searchingObject == null)
        {
            Debug.LogWarning("Nothing to stop following.");
            return;
        }
        ResetCoroutine();
        searchingObject = null;
        agent.isStopped = true;
    }

    public void PickUp(PickUpObjectInteraction objectToPickUp)
    {
        MoveTo(objectToPickUp.gameObject);
        onTargetReached = () => DoPickUp(objectToPickUp);
    }

    public void Drop()
    {
        if (pickedUpObject == null)
        {
            Debug.Log("Nothing to drop...");
            return;
        }

        var pickUpComponent = pickedUpObject.GetComponent<PickUpObjectInteraction>();
        pickUpComponent.Interact();
        pickedUpObject = null;
    }

    void DoPickUp(PickUpObjectInteraction objectToPickUp)
    {
        pickedUpObject = objectToPickUp.gameObject;
        objectToPickUp.Interact(holdPoint);
        searchingObject = null;
    }


    bool HasUnreachableTargetMoved()
    {
        if (lastTargetUnreachablePosition == null)
        {
            lastTargetUnreachablePosition = searchingObject.transform.position;
        }

        // Check if the target has moved significantly
        if (Vector3.Distance(lastTargetUnreachablePosition.Value, searchingObject.transform.position) > hysteresisBuffer)
        {
            lastTargetUnreachablePosition = searchingObject.transform.position;
            // log only once
            if (pathCoroutine == null)
                Debug.Log("Target moved");

            return true;
        }

        return false;
    }

    void ResetCoroutine()
    {
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
            pathCoroutine = null;
            Debug.Log("Coroutine stopped");
        }
    }

    IEnumerator MoveToTargetCoroutine()
    {
        NavMeshPath path = new NavMeshPath();

        while (true)
        {
            if (searchingObject == null)
                yield break;

            // thanks to this function, path should be ALWAYS calculated immediately
            // it means that agent should not try to reach an object that is unreachable (like following object moving behind a wall)
            agent.CalculatePath(searchingObject.transform.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                targetUnreachable = false;
                agent.path = path;
            }
            else
            {
                Debug.LogWarning("Path is invalid. Target unreachable");
                agent.isStopped = true;
                targetUnreachable = true;
                ResetCoroutine();
            }

            // Wait for a short time before checking again
            yield return new WaitForSeconds(pathCalculationInterval);
        }
    }
}
