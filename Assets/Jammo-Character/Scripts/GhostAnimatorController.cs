using UnityEngine;
using UnityEngine.AI;

public class GhostAnimatorController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GhostController ghostController;

    [SerializeField] float desiredRotationSpeed = 0.1f;

    [Header("Animation Smoothing")]
    [Range(0, 1f)][SerializeField] float HorizontalAnimSmoothTime = 0.15f;
    [Range(0, 1f)]  [SerializeField] float VerticalAnimTime = 0.15f;
    [Range(0, 1f)]  [SerializeField] float StartAnimTime = 0.2f;
    [Range(0, 1f)]  [SerializeField] float StopAnimTime = 0.25f;
    [Range(0, 1f)][SerializeField] float StartCarryAnimTime = 0.4f;
    [Range(0, 1f)][SerializeField] float StopCarryAnimTime = 0.25f;

    Vector3 previousPosition;
    float speed;
    Vector3 desiredMoveDirection;

    float startTime;
    float stopTime;

    void Start()
    {
        previousPosition = transform.position;
        ghostController.onPickingUp += OnPickUp;
        ghostController.onDropped += OnDrop;

        startTime = StartAnimTime;
        stopTime = StopAnimTime;
    }

    void OnDestroy()
    {
        ghostController.onPickingUp -= OnPickUp;
        ghostController.onDropped -= OnDrop;
    }

    void Update()
    {
        UpdateAnimationAndRotation();
    }

    void UpdateAnimationAndRotation()
    {
        Vector3 velocity = agent.velocity;
        speed = velocity.magnitude;

        Vector3 flatVelocity = new Vector3(velocity.x, 0, velocity.z);
        if (flatVelocity.sqrMagnitude > 0.01f)
        {
            desiredMoveDirection = flatVelocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, desiredRotationSpeed);
        }

        float normalizedSpeed = Mathf.Clamp(agent.velocity.magnitude / agent.speed, 0f, 1f);
        anim.SetFloat("Blend", normalizedSpeed, speed > 0.01f ? startTime : stopTime, Time.deltaTime);
    }

    void OnPickUp()
    {
        anim.SetTrigger("PickUp");
        startTime = StartCarryAnimTime;
        stopTime = StopCarryAnimTime;
    }

    void OnDrop()
    {
        anim.SetTrigger("Drop");
        startTime = StartAnimTime;
        stopTime = StopAnimTime;
    }
}
