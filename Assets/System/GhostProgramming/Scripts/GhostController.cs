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
    Vector3 targetPos;
    string ghostColor;

    float distance;
    float stillTime = 0f;
    bool targetUnreachable = false;

    const float maxStillTime = 2f;
    const float pathCalculationInterval = 0.5f;
    const float actionDelay = 0.25f;
    const float rotationSpeed = 270f; // degrees per second

    public enum InteractionDistance
    {
        Default,
        Far,
        Mid,
        Close,
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EntityManager.Instance.Register<GhostController>(this);
        SetColor();
        UpdateEntityDisplayName();
    }

    void Update()
    {
        if (target != null)
            targetPos = target.transform.position;

        RotateTowards(targetPos);

        // If there is no target - do nothing
        if (target == null)
            return;

        // If target is unreachable - stop path finding
        if (targetUnreachable)
        {
            ResetState();
            onTargetUnreachable?.Invoke();
            return;
        }

        // Calculate distance to the target
        distance = Vector3.Distance(agent.transform.position, target.transform.position);

        // If target is reached - stop path finding
        if (distance <= agent.stoppingDistance)
        {
            ResetState();
            onTargetReached?.Invoke();
            return;
        }

        // Start path finding
        if (distance > agent.stoppingDistance && pathCoroutine == null)
        {
            pathCoroutine ??= StartCoroutine(MoveToTargetCoroutine());
            return;
        }

        // Check if the ghost is still - if it is, interrupt the action after some time
        if (agent.velocity == Vector3.zero)
        {
            stillTime += Time.deltaTime;
            if (stillTime >= maxStillTime)
            {
                Debug.LogWarning("Ghost stood still too long — interrupting");
                ResetState();
                onTargetReached?.Invoke();
                return;
            }
        }
        else
        {
            stillTime = 0f;
        }
    }

    public async Task<bool> MoveTo(GameObject newSearchingObject, InteractionDistance dist, CancellationToken cancelToken, ExecutionResult result)
    {
        if (newSearchingObject == null)
        {
            Debug.LogWarning("New searching object is null.");
            return false;
        }

        await WaitForSeconds(actionDelay, cancelToken);
        taskCS = new TaskCompletionSource<bool>();

        agent.stoppingDistance = GetInteractionDistance(dist);

        // Reset target
        target = newSearchingObject;
        targetUnreachable = false;

        // Cancellation handling
        CancellationTokenRegistration cancelRegistration = cancelToken.Register(() =>
        {
            Stop();
            onTargetReached = null;
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
        if (objectToPickUp.gameObject == pickedUpObject?.gameObject)
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
        bool reachedDest = await MoveTo(objectToPickUp.gameObject, InteractionDistance.Far, cancelToken, result);

        if (!reachedDest)
        {
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

    public override void UpdateEntityDisplayName()
    {
        entityName = "Ghost";
        entityNameSuffix = ghostColor.ToString();
        base.UpdateEntityDisplayName();
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
        if (Vector3.Distance(lastTargetUnreachablePosition.Value, target.transform.position) > agent.stoppingDistance)
        {
            lastTargetUnreachablePosition = target.transform.position;
            // log only once
            if (pathCoroutine == null)
                Debug.Log("---Target moved---");

            return true;
        }

        // Check if the agent has moved significantly
        if (Vector3.Distance(lastMyUnreachablePosition.Value, agent.transform.position) > agent.stoppingDistance)
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
        stillTime = 0f;
    }

    IEnumerator MoveToTargetCoroutine()
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 targetPos = Vector3.negativeInfinity;

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
                targetUnreachable = true;
                yield break;
            }

            // recalculate path only when target changed its position
            if (targetPos != target.transform.position)
            {
                targetPos = target.transform.position;

                // thanks to this function, path should be ALWAYS calculated immediately
                // it means that agent should not try to reach an object that is unreachable (like following object moving behind a wall)
                agent.CalculatePath(target.transform.position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    targetUnreachable = false;
                    agent.isStopped = false;
                    agent.ResetPath();
                    agent.SetPath(path);
                }
                else
                {
                    targetUnreachable = true;
                    yield break;
                }
            }

            // Wait for a short time before trying to recalculate path in case the target is moving
            yield return new WaitForSeconds(pathCalculationInterval);
        }
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }

    void SetColor()
    {
        Color color;
        float intensity = 2f;

        switch (entityId)
        {
            case 1:
                color = Color.cyan * intensity;
                ghostColor = "<b>Blue</b>";
                break;
            case 2:
                color = Color.green * intensity;
                ghostColor = "<b>Green</b>";
                break;
            case 3:
                color = Color.red * intensity;
                ghostColor = "<b>Red</b>";
                break;
            case 4:
                color = Color.yellow * intensity;
                ghostColor = "<b>Yellow</b>";
                break;
            case 5:
                color = Color.magenta * intensity;
                ghostColor = "<b>Magenta</b>";
                break;
            default:
                color = Color.white * intensity;
                ghostColor = "<b>White</b>";
                break;
        }

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
            InteractionDistance.Mid => 2f,
            InteractionDistance.Close => 0.75f,
            InteractionDistance.Default => 1f,
            _ => 1f
        };
    }
}
