using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RailPath : MonoBehaviour
{
    [Range(10, 200)] public int samplesPerSegment = 50;

    private List<Vector3> allPoints = new List<Vector3>();
    private List<float> segmentLengths = new List<float>();
    private bool needsRebuild = true;
    public float totalLength { get; private set; }
    public bool closedLoop = true;

    void OnValidate() => needsRebuild = true;
    void Awake() => RebuildPath();

    void RebuildPath()
    {
        allPoints.Clear();
        segmentLengths.Clear();
        totalLength = 0f;

        Transform track = transform.Find("Track");
        if (!track) return;

        // Zbierz wszystkie punkty z całej trasy
        foreach (Transform piece in track)
        {
            Transform points = piece.Find("Points");
            if (!points || points.childCount < 1) continue;

            for (int i = 0; i < points.childCount; i++)
            {
                allPoints.Add(points.GetChild(i).position);
            }
        }

        // Dodaj punkty dla zamkniętej pętli
        if (closedLoop && allPoints.Count > 3)
        {
            allPoints.Add(allPoints[0]);
            allPoints.Add(allPoints[1]);
            allPoints.Add(allPoints[2]);
        }

        // Oblicz długości segmentów
        for (int i = 0; i < allPoints.Count - 3; i++)
        {
            float length = CalculateSegmentLength(i);
            segmentLengths.Add(length);
            totalLength += length;
        }

        needsRebuild = false;
    }

    float CalculateSegmentLength(int segmentIndex)
    {
        float length = 0f;
        Vector3 prev = GetCatmullRomPosition(segmentIndex, 0f);

        for (int s = 1; s <= samplesPerSegment; s++)
        {
            float t = s / (float)samplesPerSegment;
            Vector3 point = GetCatmullRomPosition(segmentIndex, t);
            length += Vector3.Distance(prev, point);
            prev = point;
        }
        return length;
    }

    Vector3 GetCatmullRomPosition(int segmentIndex, float t)
    {
        int p0 = segmentIndex;
        int p1 = segmentIndex + 1;
        int p2 = segmentIndex + 2;
        int p3 = segmentIndex + 3;

        // Handle looping
        if (closedLoop)
        {
            p0 %= allPoints.Count;
            p1 %= allPoints.Count;
            p2 %= allPoints.Count;
            p3 %= allPoints.Count;
        }
        else
        {
            p0 = Mathf.Clamp(p0, 0, allPoints.Count - 1);
            p1 = Mathf.Clamp(p1, 0, allPoints.Count - 1);
            p2 = Mathf.Clamp(p2, 0, allPoints.Count - 1);
            p3 = Mathf.Clamp(p3, 0, allPoints.Count - 1);
        }

        return UniformCatmullRom(
            allPoints[p0],
            allPoints[p1],
            allPoints[p2],
            allPoints[p3],
            t
        );
    }

    Vector3 UniformCatmullRom(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * (
            (2f * P1) +
            (-P0 + P2) * t +
            (2f * P0 - 5f * P1 + 4f * P2 - P3) * t2 +
            (-P0 + 3f * P1 - 3f * P2 + P3) * t3
        );
    }

    public Vector3 GetPosition(float t)
    {
        if (needsRebuild) RebuildPath();
        if (allPoints.Count < 4) return Vector3.zero;

        // Nowa implementacja z obsługą zawijania
        if (closedLoop)
        {
            t = Mathf.Repeat(t, 1f);
        }
        else
        {
            t = Mathf.Clamp01(t);
        }

        float targetDistance = t * totalLength;
        float accumulated = 0f;

        for (int i = 0; i < segmentLengths.Count; i++)
        {
            if (accumulated + segmentLengths[i] >= targetDistance)
            {
                float segmentT = (targetDistance - accumulated) / segmentLengths[i];
                return GetCatmullRomPosition(i, segmentT);
            }
            accumulated += segmentLengths[i];
        }

        return GetCatmullRomPosition(segmentLengths.Count - 1, 1f);
    }

    void OnDrawGizmos()
    {
        if (needsRebuild) RebuildPath();
        if (allPoints.Count < 4) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < segmentLengths.Count; i++)
        {
            Vector3 prev = GetCatmullRomPosition(i, 0f);
            for (int j = 1; j <= samplesPerSegment; j++)
            {
                Vector3 next = GetCatmullRomPosition(i, j / (float)samplesPerSegment);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}