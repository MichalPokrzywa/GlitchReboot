using UnityEngine;

public class TrainController : MonoBehaviour
{
    public RailPath railPath;
    public float maxSpeed = 5f;
    public float acceleration = 1f;
    public float deceleration = 2f;
    public bool drive = false;
    public Vector3 Velocity { get; private set; }
    [HideInInspector] public float pathProgress;

    private float currentSpeed;
    private Vector3 lastPosition;

    void Start()
    {
        pathProgress = 0f;
        currentSpeed = 0f;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!railPath) return;

        // Aktualizacja prêdkoœci
        float targetSpeed = drive ? maxSpeed : 0f;
        float accelerationRate = drive ? acceleration : deceleration;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            accelerationRate * Time.deltaTime
        );

        // Aktualizacja pozycji
        if (currentSpeed > 0.01f)
        {
            float distance = currentSpeed * Time.deltaTime;
            pathProgress = CalculateNewProgress(pathProgress, distance);

            Vector3 newPos = railPath.GetPosition(pathProgress);
            Vector3 direction = (newPos - lastPosition).normalized;

            transform.position = newPos;
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);

            Velocity = (newPos - lastPosition) / Time.deltaTime;
            lastPosition = newPos;
        }
    }

    float CalculateNewProgress(float currentProgress, float distance)
    {
        if (railPath.closedLoop)
        {
            return (currentProgress + distance / railPath.totalLength) % 1f;
        }
        else
        {
            float newProgress = currentProgress + distance / railPath.totalLength;
            return Mathf.Clamp01(newProgress);
        }
    }

    // Dodatkowa metoda do rêcznego sterowania
    public void ToggleDrive(bool newState)
    {
        drive = newState;
    }

    public RailPath GetRailPath()
    {
        return railPath;
    }
}