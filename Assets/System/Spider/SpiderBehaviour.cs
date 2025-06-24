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
        public Emotion emotion;
        public AudioClip voiceClip;
        [Header("Reaching this target results in activating a game mechanic")]
        public MechanicType activateMechanic;
        public bool teleportToNext = false;
        public bool overrideStep = false;
        [Header("If audio clip not included, use this variable")]
        public float waitTime = 0f;
        [TextArea(5, 10)] public string speech;

        [HideInInspector] public bool isDone = false;
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
    [SerializeField] List<TargetData> targetData = new List<TargetData>();

    [Header("Glitch Settings")]
    [SerializeField] float glitchDuration = 0.5f;  // seconds of glitch effect

    public bool isTalking = false;

    int currentTargetIndex = 0;
    bool isWaiting = false;
    bool initialized = false;
    bool isMovementActive = false;

    const float targetDistThreshold = 0.5f;
    const float spiderSpeed = 1.5f;

    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<FirstPersonController>().gameObject;

        if (eyes.target == null)
            eyes.target = player.transform;

        if (targetData.Count == 0)
        {
            Debug.LogWarning("No targets assigned to SpiderBehaviour. Staying in place");
            isWaiting = true;
            return;
        }

        // Teleport spider to first waypoint at start
        currentTargetIndex = 0;
        Transform first = targetData[0].target;
        transform.position = first.position;
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
        if (data.overrideStep)
        {
            TriggerMechanicIfNeeded(currentTargetIndex);
            data.overrideStep = false;
            StopAllCoroutines();
            StartCoroutine(SkipToSteps(data));
        }
        else if (!isTalking)
        {
            StartCoroutine(HandleTargetReached(data));
        }
    }

    IEnumerator SkipToSteps(TargetData data)
    {
        currentTargetIndex = targetData.FindIndex(td => td == data);
        for (int i = currentTargetIndex-1; i > 0; i--)
        {
            targetData[i].isDone = true;
            TriggerMechanicIfNeeded(i);
        }

        isWaiting = true;
        isTalking = false;
        isMovementActive = false;
        glitchSwitcher.ApplyGlitch(true);
        spiderAnim.enabled = false;
        transform.position = data.target.position;
        SetEmotion(data.emotion);
        spiderAnim.enabled = true;
        yield return new WaitForSeconds(glitchDuration);
        glitchSwitcher.ApplyGlitch(false);
        StartCoroutine(HandleTargetReached(data));
    }

    IEnumerator HandleTargetReached(TargetData data)
    {
        // Check if the data is valid and not already done
        if (data == null || data.isDone || targetData[currentTargetIndex] != data)
            yield break;

        if (!initialized)
        {
            initialized = true;
            yield return new WaitForSeconds(startWaitTime);
        }

        isWaiting = false;
        data.overrideStep = false;
        isTalking = true;

        // Set wait time based on voice clip or specified wait time
        float waitTime = data.voiceClip ? data.voiceClip.length : data.waitTime;

        if (!string.IsNullOrEmpty(data.speech))
            NarrativeSystem.Instance.SetText(data.speech, waitTime);

        if (data.voiceClip)
        {
            NarrativeSystem.Instance.Play(data.voiceClip);
            yield return new WaitUntil(() => !NarrativeSystem.Instance.IsPlaying);
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        data.isDone = true;
        isMovementActive = true;
        TriggerMechanicIfNeeded(currentTargetIndex);

        // Check if there is a next target
        int nextIndex = currentTargetIndex + 1;
        if (nextIndex >= targetData.Count)
        {
            ResetStates();
            yield break;
        }

        TargetData nextData = targetData[nextIndex];

        if (data.teleportToNext)
        {
            glitchSwitcher.ApplyGlitch(true);
            yield return new WaitForSeconds(glitchDuration);
            spiderAnim.enabled = false;
            transform.position = nextData.target.position;
            SetEmotion(nextData.emotion);
            spiderAnim.enabled = true;
            yield return new WaitForSeconds(glitchDuration);
            glitchSwitcher.ApplyGlitch(false);
        }
        else
        {
            // Walk towards the next target
            while (Vector3.Distance(transform.position, nextData.target.position) > targetDistThreshold)
            {
                MoveTowards(nextData.target.position, spiderSpeed);
                RotateTowards(nextData.target.position);
                yield return null;
            }
            SetEmotion(nextData.emotion);
        }

        currentTargetIndex = nextIndex;
        ResetStates();

        if (!nextData.target.TryGetComponent(out BoxCollider box))
        {
            StartCoroutine(HandleTargetReached(nextData));
        }
    }

    void TriggerMechanicIfNeeded(int index)
    {
        if (targetData[index].activateMechanic != MechanicType.None)
            MechanicsManager.Instance.Enable(targetData[index].activateMechanic);
    }

    void ResetStates()
    {
        isWaiting = true;
        isTalking = false;
        isMovementActive = false;
    }

    void MoveTowards(Vector3 targetPos, float speed)
    {
        float step = spiderSpeed * Time.deltaTime;
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
                smile.transform.localRotation = Quaternion.Euler(0, 0, 0);
            else if (emotion == Emotion.Sad)
                smile.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }
}
