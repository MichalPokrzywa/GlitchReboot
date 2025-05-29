using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehaviour : MonoBehaviour
{
    public enum Emotion
    {
        Happy,
        Angry,
        Sad
    }

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
        public Emotion emotion;
        public string speech;
        public int voiceKey;
    }

    [Header("References")]
    [SerializeField] SpiderProceduralAnimation spiderAnim;
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> eyebrowsHappy;
    [SerializeField] List<GameObject> eyebrowsAngry;
    [SerializeField] RectTransform smile;

    [Header("Movement Settings")]
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] bool loop = false;
    [SerializeField] List<TargetData> targetData = new List<TargetData>();

    int currentTargetIndex = 0;
    bool playAtStart = false;
    bool isWaiting = false;
    bool isMovementActive = true;
    bool lookAtTarget = false;

    const float targetDistThreshold = 0.01f;

    void Start()
    {
        if (targetData.Count > 0)
        {
            playAtStart = true;

            SetAnimationData(targetData[0]);
            SetEmotion(targetData[0].emotion);
            if (!string.IsNullOrEmpty(targetData[0].speech))
                NarrativeSystem.instance.SetText(targetData[0].speech);
            if (targetData[0].voiceKey != 0)
                NarrativeSystem.instance.Play(targetData[0].voiceKey);
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

        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist > targetDistThreshold * 50f)
            RotateTowards(targetPos);

        if (dist < targetDistThreshold)
            StartCoroutine(WaitAtWaypoint(current));
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

    void SetEmotion(Emotion emotion)
    {
        if (emotion == Emotion.Angry)
        {
            foreach (var eyebrow in eyebrowsHappy)
                eyebrow.SetActive(false);
            foreach (var eyebrow in eyebrowsAngry)
                eyebrow.SetActive(true);

            smile.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            foreach (var eyebrow in eyebrowsHappy)
                eyebrow.SetActive(true);
            foreach (var eyebrow in eyebrowsAngry)
                eyebrow.SetActive(false);

            if (emotion == Emotion.Happy)
            {
                smile.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (emotion == Emotion.Sad)
            {
                smile.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
        }
    }

    void SetAnimationData(TargetData data)
    {
        spiderAnim.smoothness = data.animSmoothness;
        spiderAnim.minBodyHeight = data.minBodyHeight;
    }

    IEnumerator WaitAtWaypoint(TargetData data)
    {
        isWaiting = true;

        if (!playAtStart)
        {
            if (!string.IsNullOrEmpty(data.speech))
                NarrativeSystem.instance.SetText(data.speech);
            if (data.voiceKey != 0)
                NarrativeSystem.instance.Play(data.voiceKey);
        }

        playAtStart = false;

        yield return new WaitForSeconds(data.waitTime);

        currentTargetIndex++;
        if (currentTargetIndex >= targetData.Count)
        {
            currentTargetIndex = 0;
            if (!loop)
            {
                isMovementActive = false;
            }
        }

        isWaiting = false;

        if (currentTargetIndex == 0 && !loop)
            yield break;

        SetAnimationData(targetData[currentTargetIndex]);
        SetEmotion(targetData[currentTargetIndex].emotion);

    }
}
