using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostController : MonoBehaviour
{
    [SerializeField] Transform holdPoint;

    Action onTargetReached;
    TaskCompletionSource<bool> taskCS;

    NavMeshAgent agent;
    GameObject searchingObject;
    GameObject pickedUpObject;

    Coroutine pathCoroutine;
    Vector3? lastTargetUnreachablePosition = null;
    Vector3? lastMyUnreachablePosition = null;

    float distance;
    float interactionDistance = 2f;
    bool hasReachedTarget = false;
    bool targetUnreachable = false;
    bool stopFollowingWhenReached = true;

    const float pathCalculationInterval = 0.25f;
    const float hysteresisBuffer = 0.2f;

    public enum InteractionDistance
    {
        Close,
        Far
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        EntityManager.instance.Register(gameObject);
    }

    void Update()
    {
        if (searchingObject == null)
        {
            return;
        }

        // Check if we should consider path recalculation
        if (targetUnreachable && !IsPathRecalculationNeeded())
            return;

        distance = Vector3.Distance(agent.transform.position, searchingObject.transform.position);

        // If target is reached - stop path finding
        if (!hasReachedTarget && distance < interactionDistance)
        {
            agent.isStopped = true;
            hasReachedTarget = true;
            agent.ResetPath();
            Debug.Log("Target reached");
            ResetCoroutine();
            if (stopFollowingWhenReached)
            {
                searchingObject = null;
            }
            onTargetReached?.Invoke();
            onTargetReached = null;
            return;
        }
        // if target was reached already, but it moved away - start path finding again
        if (distance > interactionDistance + hysteresisBuffer)
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

    public async Task<bool> MoveTo(GameObject newSearchingObject)
    {
        taskCS = new TaskCompletionSource<bool>();

        if (newSearchingObject == null)
        {
            Debug.LogWarning("New searching object is null.");
            return false;
        }

        InteractionDistance dist = InteractionDistance.Far;
        var marker = newSearchingObject.GetComponent<MarkerScript>();
        if (marker != null)
            dist = InteractionDistance.Close;

        interactionDistance = agent.stoppingDistance = GetInteractionDistance(dist);
        searchingObject = newSearchingObject;
        targetUnreachable = false;
        hasReachedTarget = false;

        onTargetReached = () =>
        {
            taskCS.TrySetResult(true);
            onTargetReached = null;
        };

        return await taskCS.Task;
    }

    public async Task WaitForSeconds(float time, CancellationToken token)
    {
        int milliseconds = Mathf.RoundToInt(time * 1000f);
        try
        {
            await Task.Delay(milliseconds, token);
        }
        catch (TaskCanceledException)
        {
            Debug.Log("WaitForSeconds canceled");
        }
    }

    public void Stop()
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

    public async Task<bool> PickUp(PickUpObjectInteraction objectToPickUp)
    {
        if (objectToPickUp == null)
        {
            Debug.LogWarning("Object to pick up is null.");
            return false;
        }

        bool reachedDest = await MoveTo(objectToPickUp.gameObject);
        if (!reachedDest)
        {
            Debug.LogWarning("Failed to reach the object.");
            return false;
        }
        DoPickUp(objectToPickUp);
        return true;
    }

    public bool Drop()
    {
        if (pickedUpObject == null)
        {
            Debug.Log("Nothing to drop...");
            return false;
        }

        var pickUpComponent = pickedUpObject.GetComponent<PickUpObjectInteraction>();
        pickUpComponent.DropMe();
        pickedUpObject = null;
        return true;
    }

    void DoPickUp(PickUpObjectInteraction objectToPickUp)
    {
        pickedUpObject = objectToPickUp.gameObject;
        objectToPickUp.PickMeUp(holdPoint, null);
        searchingObject = null;
    }

    bool IsPathRecalculationNeeded()
    {
        lastTargetUnreachablePosition ??= searchingObject.transform.position;
        lastMyUnreachablePosition ??= agent.transform.position;

        // Check if the target has moved significantly
        if (Vector3.Distance(lastTargetUnreachablePosition.Value, searchingObject.transform.position) > hysteresisBuffer)
        {
            lastTargetUnreachablePosition = searchingObject.transform.position;
            // log only once
            if (pathCoroutine == null)
                Debug.Log("Target moved");

            return true;
        }

        // Check if the agent has moved significantly
        if (Vector3.Distance(lastMyUnreachablePosition.Value, agent.transform.position) > hysteresisBuffer)
        {
            lastMyUnreachablePosition = agent.transform.position;
            // log only once
            if (pathCoroutine == null)
                Debug.Log("Agent moved");

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
            // if there is no object to follow - stop coroutine
            if (searchingObject == null)
                yield break;

            // if agent is already on the path or on off-mesh link - let him finish
            if (agent.isOnOffMeshLink)
            {
                yield return null;
                continue;
            }

            // thanks to this function, path should be ALWAYS calculated immediately
            // it means that agent should not try to reach an object that is unreachable (like following object moving behind a wall)
            agent.CalculatePath(searchingObject.transform.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                targetUnreachable = false;
                agent.isStopped = false;
                if (!agent.hasPath || Vector3.Distance(agent.destination, searchingObject.transform.position) > 2*hysteresisBuffer)
                {
                    Debug.Log("Setting new path");
                    agent.SetPath(path);
                }
            }
            else
            {
                Debug.LogWarning("Path is invalid. Target unreachable");
                agent.ResetPath();
                agent.isStopped = true;
                targetUnreachable = true;
                ResetCoroutine();
                yield break;
            }

            // Wait for a short time before checking again
            yield return new WaitForSeconds(pathCalculationInterval);
        }
    }

    public static float GetInteractionDistance(InteractionDistance type)
    {
        return type switch
        {
            InteractionDistance.Far => 2f,
            InteractionDistance.Close => 0.3f,
            _ => 0.5f
        };
    }
}
