using UnityEngine;

public class WagonController : MonoBehaviour
{
    [Header("Settings")]
    public Transform leader;
    public float followDistance = 5f;
    public RailPath railPath;

    private float currentProgress;


    void Update()
    {
        if (leader == null || railPath == null) return;

        // Pobierz progres lidera
        float leaderProgress = GetLeaderProgress();

        // Oblicz w³asny progres
        currentProgress = leaderProgress - (followDistance / railPath.totalLength);

        // Obs³u¿ zapêtlanie
        if (railPath.closedLoop)
            currentProgress = Mathf.Repeat(currentProgress, 1f);
        else
            currentProgress = Mathf.Clamp01(currentProgress);

        // Ustaw pozycjê i rotacjê
        UpdatePosition();
        UpdateRotation();
    }

    float GetLeaderProgress()
    {
        if (leader.TryGetComponent<TrainController>(out var train))
            return train.pathProgress;

        if (leader.TryGetComponent<WagonController>(out var wagon))
            return wagon.currentProgress;

        return 0f;
    }

    void UpdatePosition()
    {
        transform.position = railPath.GetPosition(currentProgress);
    }

    void UpdateRotation()
    {
        float lookAhead = Mathf.Clamp(currentProgress + 0.001f, 0f, 1f);
        Vector3 direction = railPath.GetPosition(lookAhead) - transform.position;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}