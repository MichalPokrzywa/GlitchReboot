using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RailPath : MonoBehaviour
{
    [Range(10, 200)] public int samplesPerSegment = 50;
    private List<List<Vector3>> segments = new List<List<Vector3>>();
    private List<float> segmentLengths = new List<float>();
    private bool needsRebuild = true;

    void OnValidate() => needsRebuild = true;
    void Awake() => RebuildSegments();

    void RebuildSegments()
    {
        segments.Clear();
        segmentLengths.Clear();

        Transform track = transform.Find("Track");
        if (!track) return;

        foreach (Transform piece in track)
        {
            Transform points = piece.Find("Points");
            if (!points || points.childCount != 5) continue;

            List<Vector3> segment = new List<Vector3>();
            for (int i = 0; i < 5; i++)
                segment.Add(points.GetChild(i).position);

            segments.Add(segment);
            segmentLengths.Add(CalculateSegmentLength(segments.Count - 1));
        }
        needsRebuild = false;
    }

    float CalculateSegmentLength(int segmentIndex)
    {
        List<Vector3> segment = segments[segmentIndex];
        float length = 0f;
        Vector3 prev = segment[0];

        for (int i = 0; i < 4; i++) // Dla p0-p1, p1-p2, p2-p3, p3-p4
        {
            for (int s = 0; s <= samplesPerSegment; s++)
            {
                float t = s / (float)samplesPerSegment;
                Vector3 point = GetPoint(segmentIndex, i, t);
                length += Vector3.Distance(prev, point);
                prev = point;
            }
        }
        return length;
    }

    Vector3 GetPoint(int segmentIndex, int subSegment, float t)
    {
        List<Vector3> current = segments[segmentIndex];
        bool isLastSegment = segmentIndex == segments.Count - 1;

        Vector3 P0 = current[subSegment];
        Vector3 P1 = current[subSegment + 1];

        Vector3 P2 = subSegment + 2 < current.Count ?
            current[subSegment + 2] :
            isLastSegment ?
                segments[0][1] : // Połączenie ostatniego p4 z pierwszym p0
                segments[segmentIndex + 1][1];

        Vector3 P3 = subSegment + 3 < current.Count ?
            current[subSegment + 3] :
            isLastSegment ?
                segments[0][2] :
                segments[segmentIndex + 1][2];

        return UniformCatmullRom(P0, P1, P2, P3, t);
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

    public Vector3 GetPosition(int segmentIndex, float t)
    {
        if (needsRebuild) RebuildSegments();
        if (segmentIndex < 0 || segmentIndex >= segments.Count) return Vector3.zero;

        t = Mathf.Clamp01(t) * 4f; // 4 podsegmenty
        int subSegment = Mathf.FloorToInt(t);
        float localT = t - subSegment;

        return GetPoint(segmentIndex, subSegment, localT);
    }

    public float GetSegmentLength(int index) => segmentLengths[index];
    public int SegmentCount => segments.Count;

    void OnDrawGizmos()
    {
        if (needsRebuild) RebuildSegments();
        if (segments.Count == 0) return;

        Gizmos.color = Color.yellow;
        for (int s = 0; s < segments.Count; s++)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 prev = GetPoint(s, i, 0f);
                for (int j = 1; j <= samplesPerSegment; j++)
                {
                    Vector3 next = GetPoint(s, i, j / (float)samplesPerSegment);
                    Gizmos.DrawLine(prev, next);
                    prev = next;
                }
            }

            // Rysuj połączenie p4 z następnym p0
            if (s == segments.Count - 1)
            {
                Vector3 last = GetPoint(s, 3, 1f);
                Vector3 first = GetPoint(0, 0, 0f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(last, first);
            }
        }
    }
}