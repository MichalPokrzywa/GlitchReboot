using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderProceduralAnimation : MonoBehaviour
{
    public Transform[] legTargets;
    public float stepSize = 1f;
    public int smoothness = 1;
    public float stepHeight = 0.1f;
    public bool bodyOrientation = true;
    public float orientationLerpSpeed = 5f;
    public float raycastRange = 1f; // range for ground sampling
    public float minBodyHeight = 0.5f; // minimum height from ground
    public float bodyHeightOffset = 0.5f; // height above average leg positions

    private Vector3[] defaultLegPositions; // local offsets
    private Vector3[] lastLegPositions;
    private Vector3 lastBodyUp;
    private bool[] legMoving;
    private int nbLegs;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private Vector3 lastBodyPos;

    private float velocityMultiplier = 15f;

    void Start()
    {
        lastBodyUp = transform.up;

        nbLegs = legTargets.Length;
        defaultLegPositions = new Vector3[nbLegs];
        lastLegPositions = new Vector3[nbLegs];
        legMoving = new bool[nbLegs];
        for (int i = 0; i < nbLegs; ++i)
        {
            defaultLegPositions[i] = legTargets[i].localPosition;
            // initialize last positions to ground-sampled default
            lastLegPositions[i] = SampleGround(transform.TransformPoint(defaultLegPositions[i]));
            legMoving[i] = false;
            legTargets[i].position = lastLegPositions[i];
        }
        lastBodyPos = transform.position;
    }

    IEnumerator PerformStep(int index, Vector3 targetPoint)
    {
        Vector3 startPos = lastLegPositions[index];
        for (int i = 1; i <= smoothness; ++i)
        {
            float t = i / (float)(smoothness + 1);
            legTargets[index].position = Vector3.Lerp(startPos, targetPoint, t)
                                       + transform.up * Mathf.Sin(t * Mathf.PI) * stepHeight;
            yield return new WaitForFixedUpdate();
        }
        legTargets[index].position = targetPoint;
        lastLegPositions[index] = targetPoint;
        legMoving[index] = false;
    }

    void FixedUpdate()
    {
        // smooth body velocity
        velocity = (transform.position - lastBodyPos + smoothness * lastVelocity) / (smoothness + 1f);
        if (velocity.magnitude < 0.000025f)
            velocity = lastVelocity;
        else
            lastVelocity = velocity;

        // compute desired positions: sample ground under each default offset
        Vector3[] desiredPositions = new Vector3[nbLegs];
        int indexToMove = -1;
        float maxDistance = stepSize;
        for (int i = 0; i < nbLegs; ++i)
        {
            Vector3 worldDefault = transform.TransformPoint(defaultLegPositions[i]);
            desiredPositions[i] = worldDefault;

            float dist = Vector3.ProjectOnPlane(desiredPositions[i] - lastLegPositions[i], transform.up).magnitude;
            if (dist > maxDistance)
            {
                maxDistance = dist;
                indexToMove = i;
            }
        }

        // keep other legs
        for (int i = 0; i < nbLegs; ++i)
            if (i != indexToMove)
                legTargets[i].position = lastLegPositions[i];

        // move one leg
        if (indexToMove != -1 && !legMoving[indexToMove])
        {
            Vector3 targetPoint = desiredPositions[indexToMove]
                + Mathf.Clamp(velocity.magnitude * velocityMultiplier, 0f, 1.5f) * (desiredPositions[indexToMove] - legTargets[indexToMove].position)
                + velocity * velocityMultiplier;

            targetPoint = SampleGround(targetPoint);
            legMoving[indexToMove] = true;
            StartCoroutine(PerformStep(indexToMove, targetPoint));
        }

        lastBodyPos = transform.position;


        // body orientation based on leg height difference (left vs right)
        if (bodyOrientation && nbLegs > 3)
        {
            // Sample ground under body to get accurate base height
            RaycastHit bodyHit;
            float groundY = transform.position.y;
            Vector3 bodyOrigin = transform.position + Vector3.up * raycastRange;
            if (Physics.Raycast(bodyOrigin, Vector3.down, out bodyHit, raycastRange * 2))
                groundY = bodyHit.point.y;

            // Compute average leg Y to float body above legs
            float avgLegY = 0f;
            for (int i = 0; i < nbLegs; i++)
                avgLegY += lastLegPositions[i].y;
            avgLegY /= nbLegs;

            // Desired body Y: follow legs and maintain min height from ground
            float desiredY = Mathf.Max(groundY + minBodyHeight, avgLegY + bodyHeightOffset);
            Vector3 newBodyPos = transform.position;
            newBodyPos.y = desiredY;
            transform.position = Vector3.Lerp(transform.position, newBodyPos, Time.fixedDeltaTime * orientationLerpSpeed);

            // Compute roll (Z-axis) from left/right legs
            // Assuming legTargets[0], legTargets[2] are left, [1], [3] are right
            float leftAvgY = (lastLegPositions[0].y + lastLegPositions[2].y) * 0.5f;
            float rightAvgY = (lastLegPositions[3].y + lastLegPositions[1].y) * 0.5f;
            float rollDiff = leftAvgY - rightAvgY;
            float rollAngle = Mathf.Clamp(rollDiff * 50f, -100f, 100f);

            // Compute pitch (X-axis) from front/back legs
            // Assuming legTargets[0], legTargets[1] are front, [2], [3] are back
            float frontAvgY = (lastLegPositions[0].y + lastLegPositions[3].y) * 0.5f;
            float backAvgY = (lastLegPositions[1].y + lastLegPositions[2].y) * 0.5f;
            float pitchDiff = frontAvgY - backAvgY;
            float pitchAngle = Mathf.Clamp(pitchDiff * 50f, -100f, 100f);

            // Preserve current yaw
            float currentYaw = transform.eulerAngles.y;

            // Apply combined pitch and roll
            Quaternion targetRot = Quaternion.Euler(pitchAngle, currentYaw, rollAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * orientationLerpSpeed);

            lastBodyUp = transform.up;
        }

    }

    /// <summary>
    /// Raycasts down from point by raycastRange and returns the hit point or original point if none.
    /// </summary>
    Vector3 SampleGround(Vector3 point)
    {
        RaycastHit hit;
        Vector3 origin = point + Vector3.up * raycastRange;
        if (Physics.Raycast(origin, Vector3.down, out hit, raycastRange * 2))
            return hit.point;
        return point;
    }
    void MaintainMinimumHeight()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * raycastRange;
        if (Physics.Raycast(origin, Vector3.down, out hit, raycastRange * 2))
        {
            float currentHeight = hit.distance;
            float offset = minBodyHeight - currentHeight;
            if (offset > 0f)
                transform.position += Vector3.up * offset;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (legTargets == null) return;
        for (int i = 0; i < nbLegs; ++i)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(legTargets[i].position, 0.05f);
            Gizmos.color = Color.green;
            Vector3 basePos = Application.isPlaying ? SampleGround(transform.TransformPoint(defaultLegPositions[i])) : transform.TransformPoint(defaultLegPositions[i]);
            Gizmos.DrawWireSphere(basePos, stepSize);
        }
    }
}
