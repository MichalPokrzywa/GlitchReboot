using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    [SerializeField] GameObject searchingObject;

    // for cubes: 2f, platforms: 0.1f
    public float interactionDistance = 2f;

    NavMeshAgent agent;
    Coroutine pathCoroutine;
    GameObject lastTarget = null;
    float distance;
    bool hasReachedTarget = false;
    bool targetUnreachable = false;
    Vector3? lastTargetUnreachablePosition = null;

    const float pathCalculationInterval = 0.5f;
    const float hysteresisBuffer = 0.2f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (searchingObject == null)
        {
            Debug.LogWarning("Searching object is not assigned.");
            return;
        }

        // Update targets if needed
        if (searchingObject != lastTarget)
        {
            lastTarget = searchingObject;
            targetUnreachable = false;
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
        if (!hasReachedTarget && distance < interactionDistance)
        {
            agent.isStopped = true;
            hasReachedTarget = true;
            ResetCoroutine();
        }
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

    bool HasUnreachableTargetMoved()
    {
        if (lastTargetUnreachablePosition == null)
        {
            lastTargetUnreachablePosition = searchingObject.transform.position;
        }

        // Check if the target has moved significantly
        if (Vector3.Distance(lastTargetUnreachablePosition.Value, searchingObject.transform.position) > 0.1f)
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
