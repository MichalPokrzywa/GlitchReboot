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

    int currentTargetIndex = 0;

    bool initialized = false;
    bool isCurrentlyMoving = false;
    bool isInStepRoutine = false; // HandleTargetReached() is running

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
        // Rotate towards player if not moving
        if (!isCurrentlyMoving)
        {
            RotateTowards(player.transform.position);
            return;
        }
    }

    public void OnPlayerEnterTarget(TargetData data)
    {
        int dataIndex = targetData.IndexOf(data);
        if (dataIndex < currentTargetIndex)
        {
            //Debug.LogError($"Próbujesz wejœæ w {data.target.name} ale to juz stare dzieje");
            return;
        }
        TargetData current = targetData[currentTargetIndex];

        if (!current.isDone && !isInStepRoutine && current == data)
        {
            //Debug.LogError($"Lecimy z tematem");
            StartCoroutine(HandleTargetReached(data));
            return;
        }

        // If the current target is the same as the one entered, do nothing
        if (!current.isDone && isInStepRoutine && current == data)
        {
            //Debug.LogError($"W³aœnie to przetwarzam ({data.target.name}) wiec nie bede 2 raz");
            return;
        }

        // If the current target is the same as the one entered, and we are not in a step routine, start handling it
        if (current.isDone && !isInStepRoutine && current == data)
        {
            //Debug.LogError($"Podszed³eœ wiêc kontynuujê z dialogiem w {data.target.name}");
            StartCoroutine(HandleTargetReached(data));
            return;
        }

        // If entered target is next and can override current target
        if (dataIndex > currentTargetIndex && data.overrideStep)
        {
            //Debug.LogError($"Ok mogê skipn¹æ do {data.target.name}");
            StopAllCoroutines();
            isInStepRoutine = false;
            StartCoroutine(SkipToSteps(data));
            return;
        }
    }

    IEnumerator SkipToSteps(TargetData data)
    {
        currentTargetIndex = targetData.FindIndex(td => td == data);
        for (int i = currentTargetIndex - 1; i > 0; i--)
        {
            if (targetData[i].isDone)
                continue;

            targetData[i].isDone = true;
            TriggerMechanicIfNeeded(i);
        }

        glitchSwitcher.ApplyGlitch(true);
        spiderAnim.enabled = false;
        transform.position = data.target.position;
        SetEmotion(data.emotion);
        spiderAnim.enabled = true;

        yield return new WaitForSeconds(glitchDuration);

        glitchSwitcher.ApplyGlitch(false);
        //Debug.LogError($"Po skipie lecê normalnie z: {data.target.name}");

        StartCoroutine(HandleTargetReached(data));
    }

    IEnumerator HandleTargetReached(TargetData data)
    {
        // Check if the data is valid and not already done
        if (data == null || data.isDone || targetData[currentTargetIndex] != data || isInStepRoutine)
            yield break;

        isInStepRoutine = true;

        if (!initialized)
        {
            initialized = true;
            yield return new WaitForSeconds(startWaitTime);
        }

        // Set wait time based on voice clip or specified wait time
        float waitTime = data.voiceClip ? data.voiceClip.length : data.waitTime;

        //Debug.LogError("Mówiê kwestiê i czekam...");

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
        TriggerMechanicIfNeeded(currentTargetIndex);

        // Check if there is a next target
        int nextIndex = currentTargetIndex + 1;
        if (nextIndex >= targetData.Count)
        {
            //Debug.LogError($"Koniec przechadzek");
            isInStepRoutine = false;
            yield break;
        }

        //Debug.LogError($"Skoñczy³em czekaæ, teraz idê dalej {targetData[nextIndex].target.name}");

        currentTargetIndex = nextIndex;
        TargetData nextData = targetData[nextIndex];
        isCurrentlyMoving = true;

        // Teleport to the next target
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
            //Debug.LogError($"Teleport do: {nextData.target.name}");
        }
        // Walk towards the next target
        else
        {
            while (Vector3.Distance(transform.position, nextData.target.position) > targetDistThreshold)
            {
                MoveTowards(nextData.target.position, spiderSpeed);
                RotateTowards(nextData.target.position);
                yield return null;
            }
            //Debug.LogError($"Docz³apa³em do: {nextData.target.name}");
            SetEmotion(nextData.emotion);
        }

        currentTargetIndex = nextIndex;
        isInStepRoutine = false;
        isCurrentlyMoving = false;

        // If nextTarget has no BoxCollider, go there immediately
        if (!nextData.target.TryGetComponent(out BoxCollider box))
        {
            StartCoroutine(HandleTargetReached(nextData));
            //Debug.LogError($"Lecê NATYCHMIAST do: {nextData.target.name}");
        }
    }

    void TriggerMechanicIfNeeded(int index)
    {
        if (targetData[index].activateMechanic != MechanicType.None)
            MechanicsManager.Instance.Enable(targetData[index].activateMechanic);
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
