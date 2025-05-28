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
    GameObject target;
    GameObject pickedUpObject;

    Coroutine pathCoroutine;
    Vector3? lastTargetUnreachablePosition = null;
    Vector3? lastMyUnreachablePosition = null;

    float distance;

    bool targetUnreachable = false;
    bool stopFollowingWhenReached = true;
    bool hasWaitedBeforeAction = false;

    const float pathCalculationInterval = 0.5f;
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
        // If there is no target - do nothing
        if (target == null)
            return;

        // Check if we should consider path recalculation
        // It is needed when the target is unreachable, but it might be reachable after some time (e.g. target moved behind a wall)
        if (targetUnreachable && !IsPathRecalculationNeeded())
            return;

        // Calculate distance to the target
        distance = Vector3.Distance(agent.transform.position, target.transform.position);

        // If target is reached - stop path finding
        if (distance < agent.stoppingDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();
            ResetCoroutine();
            if (stopFollowingWhenReached)
            {
                target = null;
            }
            onTargetReached?.Invoke();
            return;
        }
        // Start path finding
        if (distance > agent.stoppingDistance + hysteresisBuffer)
        {
            agent.isStopped = false;

            if (pathCoroutine == null)
            {
                pathCoroutine ??= StartCoroutine(MoveToTargetCoroutine());
                Debug.Log("--Coroutine started--");
            }
        }
    }

    public async Task<bool> MoveTo(GameObject newSearchingObject, CancellationToken cancelToken)
    {
        if (newSearchingObject == null)
        {
            Debug.LogWarning("New searching object is null.");
            return false;
        }

        bool waited = await WaitOnceBeforeAction(cancelToken);
        taskCS = new TaskCompletionSource<bool>();

        InteractionDistance dist = InteractionDistance.Far;
        var marker = newSearchingObject.GetComponent<MarkerScript>();
        if (marker != null)
            dist = InteractionDistance.Close;

        agent.stoppingDistance = GetInteractionDistance(dist);
        target = newSearchingObject;
        targetUnreachable = false;

        // Cancellation handling
        CancellationTokenRegistration cancelRegistration = cancelToken.Register(() =>
        {
            Stop();
            onTargetReached = null;
            hasWaitedBeforeAction = false;
            taskCS.TrySetResult(false);
        });

        onTargetReached = () =>
        {
            onTargetReached = null;
            taskCS.TrySetResult(true);
        };

        bool taskValue = await taskCS.Task;
        if (waited)
            hasWaitedBeforeAction = false;


        return taskValue;
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
            Debug.LogWarning("WaitForSeconds canceled");
        }
    }

    public void Stop()
    {
        if (target == null)
        {
            Debug.LogWarning("Nothing to stop following.");
            return;
        }
        ResetCoroutine();
        target = null;
        agent.isStopped = true;
        agent.ResetPath();
    }

    public async Task<bool> PickUp(PickUpObjectInteraction objectToPickUp, CancellationToken cancelToken)
    {
        if (objectToPickUp == null)
        {
            Debug.LogWarning("Object to pick up is null.");
            return false;
        }

        bool waited = await WaitOnceBeforeAction(cancelToken);

        // make sure to drop previous object
        Drop();

        bool reachedDest = await MoveTo(objectToPickUp.gameObject, cancelToken);
        if (!reachedDest)
        {
            Debug.LogWarning("Failed to reach the object.");
            return false;
        }

        DoPickUp(objectToPickUp);

        // reset waiting flag after action
        if (waited)
            hasWaitedBeforeAction = false;

        return true;
    }

    public bool Drop()
    {
        if (pickedUpObject == null)
            return false;

        var pickUpComponent = pickedUpObject.GetComponent<PickUpObjectInteraction>();
        pickUpComponent.DropMe();
        pickedUpObject = null;
        return true;
    }

    void DoPickUp(PickUpObjectInteraction objectToPickUp)
    {
        pickedUpObject = objectToPickUp.gameObject;
        objectToPickUp.PickMeUp(holdPoint, null);
        target = null;
    }

    bool IsPathRecalculationNeeded()
    {
        lastTargetUnreachablePosition ??= target.transform.position;
        lastMyUnreachablePosition ??= agent.transform.position;

        // Check if the target has moved significantly
        if (Vector3.Distance(lastTargetUnreachablePosition.Value, target.transform.position) > hysteresisBuffer)
        {
            lastTargetUnreachablePosition = target.transform.position;
            // log only once
            if (pathCoroutine == null)
                Debug.Log("---Target moved---");

            return true;
        }

        // Check if the agent has moved significantly
        if (Vector3.Distance(lastMyUnreachablePosition.Value, agent.transform.position) > hysteresisBuffer)
        {
            lastMyUnreachablePosition = agent.transform.position;
            // log only once
            if (pathCoroutine == null)
                Debug.Log("---Agent moved---");

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
            Debug.Log("--Coroutine stopped--");
        }
    }

    IEnumerator MoveToTargetCoroutine()
    {
        NavMeshPath path = new NavMeshPath();

        while (true)
        {
            // if there is no object to follow - stop coroutine
            if (target == null)
                yield break;

            // if agent is already on the path or on off-mesh link - let him finish
            if (agent.isOnOffMeshLink)
            {
                yield return null;
                continue;
            }

            // thanks to this function, path should be ALWAYS calculated immediately
            // it means that agent should not try to reach an object that is unreachable (like following object moving behind a wall)
            agent.CalculatePath(target.transform.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                targetUnreachable = false;
                agent.isStopped = false;
                if (!agent.hasPath || Vector3.Distance(agent.destination, target.transform.position) > 2*hysteresisBuffer)
                {
                    Debug.Log("--Setting path--");
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

            // Wait for a short time before trying to recalculate path in case the target is moving
            yield return new WaitForSeconds(pathCalculationInterval);
        }
    }

    async Task<bool> WaitOnceBeforeAction(CancellationToken cancelToken)
    {
        if (hasWaitedBeforeAction)
            return false;

        hasWaitedBeforeAction = true;
        await WaitForSeconds(0.5f, cancelToken);
        return true;
    }

    public static float GetInteractionDistance(InteractionDistance type)
    {
        return type switch
        {
            InteractionDistance.Far => 2.5f,
            InteractionDistance.Close => 0.3f,
            _ => 0.5f
        };
    }
}
