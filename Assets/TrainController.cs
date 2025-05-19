using UnityEngine;

public class TrainController : MonoBehaviour
{
    public RailPath railPath;
    public float speed = 5f;

    private int currentSegment;
    private float segmentProgress;
    private Vector3 lastPosition;

    void Start()
    {
        currentSegment = 0;
        segmentProgress = 0f;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!railPath || railPath.SegmentCount == 0) return;

        float distance = speed * Time.deltaTime;
        AdvancePosition(distance);

        Vector3 newPos = railPath.GetPosition(currentSegment, segmentProgress);
        Vector3 direction = (newPos - lastPosition).normalized;

        transform.position = newPos;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);

        lastPosition = newPos;
    }

    void AdvancePosition(float distance)
    {
        while (distance > 0.001f)
        {
            float remaining = railPath.GetSegmentLength(currentSegment) * (1f - segmentProgress);

            if (distance <= remaining)
            {
                segmentProgress += distance / railPath.GetSegmentLength(currentSegment);
                break;
            }

            distance -= remaining;
            currentSegment = (currentSegment + 1) % railPath.SegmentCount;
            segmentProgress = 0f;
        }
    }
}