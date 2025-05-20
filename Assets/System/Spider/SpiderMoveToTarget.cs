using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMoveToTarget : MonoBehaviour
{
    [Serializable]
    class TargetData
    {
        public Transform target;
        [Range(0, 1.5f)]
        public float speed = 1f;
        [Range(0, 15f)]
        public int animSmoothness;
        [Range(0, 1f)]
        public float minBodyHeight;
        public float waitTime = 0f;
        public string speech;
    }
    [Header("References")]
    [SerializeField] SpiderProceduralAnimation spiderAnim;
    [SerializeField] GameObject player;

    [Header("Movement Settings")]
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] bool loop = false;
    [SerializeField] List<TargetData> targetData = new List<TargetData>();

    int currentTargetIndex = 0;
    bool isWaiting = false;
    bool isMovementActive = true;
    bool lookAtTarget = false;

    const float targetDistThreshold = 0.01f;

    void Start()
    {
        if (targetData.Count > 0)
        {
            SetAnimationData(targetData[0]);
        }
        else
        {
            isMovementActive = false;
        }
    }

    void Update()
    {
        if (isWaiting || !isMovementActive)
        {
            RotateTowards(player.transform.position);
            return;
        }

        if (!isMovementActive || targetData.Count == 0)
            return;

        TargetData current = targetData[currentTargetIndex];
        Vector3 targetPos = current.target.position;

        MoveTowards(targetPos, current.speed);
        RotateTowards(targetPos);

        if (Vector3.Distance(transform.position, targetPos) < targetDistThreshold)
        {
            StartCoroutine(WaitAtWaypoint(current));
        }
    }

    void MoveTowards(Vector3 targetPos, float speed)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void SetAnimationData(TargetData data)
    {
        spiderAnim.smoothness = data.animSmoothness;
        spiderAnim.minBodyHeight = data.minBodyHeight;
    }

    IEnumerator WaitAtWaypoint(TargetData data)
    {
        isWaiting = true;

        if (!string.IsNullOrEmpty(data.speech))
        {
            NarrativeSystem.instance.SetText(data.speech);
        }

        yield return new WaitForSeconds(data.waitTime);

        currentTargetIndex++;
        if (currentTargetIndex >= targetData.Count)
        {
            currentTargetIndex = 0;
            if (!loop)
                isMovementActive = false;
            else
                SetAnimationData(targetData[currentTargetIndex]);
        }

        isWaiting = false;
    }
}
