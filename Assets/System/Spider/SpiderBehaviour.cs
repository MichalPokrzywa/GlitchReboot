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
    public class TargetData
    {
        public Transform target;
        [Range(0, 1.5f)]
        public float speed = 1f;
        [Range(0, 15f)]
        public int animSmoothness;
        [Range(0, 1f)]
        public float minBodyHeight;

        public float textDisplayAdditionalTime = 0f;
        public float waitTimeAtWaypoint = 0f;
        public Emotion emotion;
        [TextArea(5,10)]
        public string speech;

        public Scene scene;
        public int voiceKey;
        public bool teleportToNext = false;
        [HideInInspector]
        public bool isDone = false;
        public bool overrideStep = false;
    }

    [Header("References")]
    [SerializeField] SpiderProceduralAnimation spiderAnim;
    [SerializeField] GameObject player;
    [SerializeField] GlitchSwitcher glitchSwitcher;
    [SerializeField] List<GameObject> eyebrowsHappy;
    [SerializeField] List<GameObject> eyebrowsAngry;
    [SerializeField] RectTransform smile;
    [SerializeField] Eyes2DMovement eyes;

    [Header("Movement Settings")]
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float startWaitTime = 3f;
    [SerializeField] bool loop = false;
    [SerializeField] List<TargetData> targetData = new List<TargetData>();

    [Header("Glitch Settings")]
    [SerializeField] float glitchDuration = 0.5f;  // seconds of glitch effect

    int currentTargetIndex = 0;
    bool playAtStart = false;
    bool isWaiting = false;
    bool initialized = false;
    bool isMovementActive = false;
    public bool isTalking = false;

    const float targetDistThreshold = 0.01f;
    void Start()
    {
        if (player == null)
            player = FindObjectOfType<FirstPersonController>().gameObject;

        if (eyes.target == null)
            eyes.target = player.transform;

        if (targetData.Count == 0)
        {
            Debug.LogWarning("No targets assigned to SpiderBehaviour. Staying in Place");
            isWaiting = true;
            return;
        }

        // Teleport spider to first waypoint at start
        currentTargetIndex = 0;
        Transform first = targetData[0].target;
        transform.position = first.position;
        SetAnimationData(targetData[0]);
        SetEmotion(targetData[0].emotion);

        // Ensure glitch is off initially
        glitchSwitcher?.ApplyGlitch(false);

        // Assign triggers to target colliders
        foreach (var data in targetData)
        {
            TargetTrigger trigger = data.target.GetComponent<TargetTrigger>();
            if (trigger == null)
            {
                trigger = data.target.gameObject.AddComponent<TargetTrigger>();
            }

            if (data.target.TryGetComponent(out BoxCollider box))
            {
                box.isTrigger = true;
            }

            trigger.Setup(this, data);
        }
    }
    void Update()
    {
        if ((isWaiting || isTalking) && !isMovementActive)
        {
            RotateTowards(player.transform.position);
            return;
        }
    }

    public void OnPlayerEnterTarget(TargetData data)
    {
        if (!isTalking)
            StartCoroutine(HandleTargetReached(data));
        else if (data.overrideStep)
        {
            data.overrideStep = false;
            StopAllCoroutines();
            StartCoroutine(SkipToSteps(data));
        }
    }

    IEnumerator SkipToSteps(TargetData data)
    {
        currentTargetIndex = targetData.FindIndex(td => td == data);
        for (int i = currentTargetIndex-1; i > 0; i--)
        {
            targetData[i].isDone = true;
        }
        isWaiting = true;
        isTalking = false;
        isMovementActive = false;
        glitchSwitcher.ApplyGlitch(true);
        spiderAnim.enabled = false;
        transform.position = data.target.position;
        SetAnimationData(data);
        SetEmotion(data.emotion);
        spiderAnim.enabled = true;
        yield return new WaitForSeconds(glitchDuration);
        glitchSwitcher.ApplyGlitch(false);
        StartCoroutine(HandleTargetReached(data));
    }

    IEnumerator HandleTargetReached(TargetData data)
    {
        if (!initialized)
        {
            initialized = true;
            yield return new WaitForSeconds(startWaitTime);
        }

        if(data.isDone)
            yield break;
        if(targetData[currentTargetIndex] != data)
            yield break;
        isWaiting = false;
        data.overrideStep = false;
        isTalking = true;
        if (!string.IsNullOrEmpty(data.speech))
            NarrativeSystem.Instance.SetText(data.speech, data.textDisplayAdditionalTime);
        if (data.voiceKey != 0)
            NarrativeSystem.Instance.Play(data.scene, data.voiceKey);

        yield return new WaitForSeconds(data.waitTimeAtWaypoint);

        data.isDone = true;
        isMovementActive = true;

        int nextIndex = currentTargetIndex + 1;
        if (nextIndex >= targetData.Count)
        {
            if (loop) nextIndex = 0;
            else
            {
                isWaiting = true;
                isTalking = false;
                isMovementActive = false;
                yield break;
            }
        }

        TargetData nextData = targetData[nextIndex];

        if (data.teleportToNext)
        {
            glitchSwitcher.ApplyGlitch(true);
            yield return new WaitForSeconds(glitchDuration);
            spiderAnim.enabled = false;
            transform.position = nextData.target.position;
            SetAnimationData(nextData);
            SetEmotion(nextData.emotion);
            spiderAnim.enabled = true;
            yield return new WaitForSeconds(glitchDuration);
            glitchSwitcher.ApplyGlitch(false);
        }
        else
        {
            // Walk towards the next target
            while (Vector3.Distance(transform.position, nextData.target.position) > targetDistThreshold * 50f)
            {
                MoveTowards(nextData.target.position, nextData.speed);
                RotateTowards(nextData.target.position);

                yield return null;
            }

            SetAnimationData(nextData);
            SetEmotion(nextData.emotion);
        }

        currentTargetIndex = nextIndex;
        isWaiting = true;
        isTalking = false;
        isMovementActive = false;
        if (targetData[nextIndex].target.TryGetComponent(out BoxCollider box) == false)
        {
            StartCoroutine(HandleTargetReached(targetData[nextIndex]));
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
}
