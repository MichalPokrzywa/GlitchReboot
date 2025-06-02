using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using static SequenceRunner;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostController : EntityBase
{
    [SerializeField] Transform holdPoint;

    public Action onPickingUp;
    public Action onDropped;

    Action onTargetReached;
    Action onTargetUnreachable;

    TaskCompletionSource<bool> taskCS;

    NavMeshAgent agent;
    GameObject target;
    GameObject pickedUpObject;

    Coroutine pathCoroutine;
    Vector3? lastTargetUnreachablePosition = null;
    Vector3? lastMyUnreachablePosition = null;

    float distance;

    bool targetUnreachable = false;
    bool hasWaitedBeforeAction = false;

    const float pathCalculationInterval = 1f;
    const float hysteresisBuffer = 0.2f;
    const float actionDelay = 0.25f;

    public enum InteractionDistance
    {
        Close,
        Far
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdateEntityNameSuffix();
        EntityManager.instance.Register<GhostController>(this);

        float intensity = 5f;
        switch (entityId)
        {
            case 1:
                SetColor(Color.blue * intensity);
                break;
            case 2:
                SetColor(Color.green * intensity);
                break;
            default:
                SetColor(Color.red * intensity);
                break;
        }
    }

    void Update()
    {
        // If there is no target - do nothing
        if (target == null)
            return;

        // Check if we should consider path recalculation
        // It is needed when the target is unreachable, but it might be reachable after some time (e.g. target moved behind a wall)
        //if (targetUnreachable && !IsPathRecalculationNeeded())
        //    return;

        // If target is unreachable - stop recalculating path
        if (targetUnreachable)
        {
            ResetState();
            onTargetUnreachable?.Invoke();
            return;
        }

        // Calculate distance to the target
        distance = Vector3.Distance(agent.transform.position, target.transform.position);

        // If target is reached - stop recalculating path
        if (distance < agent.stoppingDistance)
        {
            ResetState();
            onTargetReached?.Invoke();
            return;
        }
        // Start path finding
        if (distance > agent.stoppingDistance + hysteresisBuffer && pathCoroutine == null && !targetUnreachable)
        {
            pathCoroutine ??= StartCoroutine(MoveToTargetCoroutine());
        }
    }

    public async Task<bool> MoveTo(GameObject newSearchingObject, CancellationToken cancelToken, ExecutionResult result)
    {
        if (newSearchingObject == null)
        {
            Debug.LogWarning("New searching object is null.");
            return false;
        }

        await WaitForSeconds(actionDelay, cancelToken);
        taskCS = new TaskCompletionSource<bool>();

        InteractionDistance dist = InteractionDistance.Far;
        var marker = newSearchingObject.GetComponent<MarkerScript>();
        if (marker != null)
            dist = InteractionDistance.Close;
        agent.stoppingDistance = GetInteractionDistance(dist);

        // Reset target
        target = newSearchingObject;
        targetUnreachable = false;

        // Cancellation handling
        CancellationTokenRegistration cancelRegistration = cancelToken.Register(() =>
        {
            Stop();
            onTargetReached = null;
            hasWaitedBeforeAction = false;
            result.errorCode = ErrorCode.Canceled;
            taskCS.TrySetResult(false);
        });

        onTargetReached = () =>
        {
            onTargetReached = null;
            taskCS.TrySetResult(true);
        };

        onTargetUnreachable = () =>
        {
            onTargetUnreachable = null;
            result.errorCode = ErrorCode.TargetUnreachable;
            taskCS.TrySetResult(false);
        };

        bool taskValue = await taskCS.Task;

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
        ResetState();
    }

    public async Task<bool> PickUp(PickUpObjectInteraction objectToPickUp, CancellationToken cancelToken, ExecutionResult result)
    {
        if (objectToPickUp == null)
        {
            result.errorCode = ErrorCode.NotPickable;
            return false;
        }

        // object is already picked
        if (objectToPickUp == pickedUpObject)
            return true;

        await WaitForSeconds(actionDelay, cancelToken);

        // make sure that object is not already picked up by someone else
        if (!objectToPickUp.IsDropped)
        {
            result.errorCode = ErrorCode.NotPickable;
            return false;
        }

        // make sure to drop previous object
        await Drop(cancelToken);

        // search for the object to pick up
        bool reachedDest = await MoveTo(objectToPickUp.gameObject, cancelToken, result);

        if (!reachedDest)
        {
            Debug.LogWarning("Failed to reach the object.");
            return false;
        }

        bool canPickUp = await DoPickUp(objectToPickUp, result);
        if (!canPickUp)
        {
            result.errorCode = ErrorCode.NotPickable;
            return false;
        }

        return true;
    }

    public async Task<bool> Drop(CancellationToken cancelToken, [CanBeNull] ExecutionResult result = null)
    {
        if (pickedUpObject == null)
        {
            if (result != null)
                result.errorCode = ErrorCode.NothingToDrop;
            return false;
        }

        await WaitForSeconds(actionDelay, CancellationToken.None);

        var pickUpComponent = pickedUpObject.GetComponent<PickUpObjectInteraction>();
        pickUpComponent.DropMe();
        pickedUpObject = null;
        onDropped?.Invoke();

        return true;
    }

    async Task<bool> DoPickUp(PickUpObjectInteraction objectToPickUp, ExecutionResult result)
    {
        // check if object is pickable
        if (!objectToPickUp.IsDropped)
            return false;

        onPickingUp?.Invoke();
        await WaitForSeconds(actionDelay, CancellationToken.None);

        // check if object is still pickable after waiting
        if (!objectToPickUp.IsDropped)
        {
            onDropped?.Invoke();
            return false;
        }

        objectToPickUp.PickMeUp(holdPoint, null);
        pickedUpObject = objectToPickUp.gameObject;

        return true;
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

    void ResetState()
    {
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
            pathCoroutine = null;
        }
        agent.isStopped = true;
        agent.ResetPath();
        target = null;
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

            var pickable = target.GetComponent<PickUpObjectInteraction>();
            if (pickable != null && !pickable.IsDropped)
            {
                Debug.LogWarning("--Target is picked up by someone else--");
                targetUnreachable = true;
                yield break;
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
                    agent.SetPath(path);
                }
            }
            else
            {
                Debug.LogWarning("--Path is invalid. Target unreachable--");
                targetUnreachable = true;
                yield break;
            }

            // Wait for a short time before trying to recalculate path in case the target is moving
            yield return new WaitForSeconds(pathCalculationInterval);
        }
    }

    void SetColor(Color color)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            rend.material.SetColor("_MainColor", color);
        }
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
