using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Eyes2DMovement : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Camera mainCamera;
    [SerializeField] List<RectTransform> pupils = new List<RectTransform>();
    [SerializeField] List<Slider> eyelids = new List<Slider>();

    [Header("Pupil Movement Settings")]
    [SerializeField] float maxPupilDistance = 50f;

    const float blinkDuration = 0.25f;
    const float blinkInterval = 2f;

    void Start()
    {
        if (eyelids.Count != 0)
            StartCoroutine(RandomBlinking());
    }

    void Update()
    {
        if (target == null || pupils.Count == 0)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        LookAtTarget();
    }

    void LookAtTarget()
    {
        foreach (var pupil in pupils)
        {
            var dir = target.transform.position - pupil.position;
            dir.Normalize();

            float x = Vector3.Dot(dir, pupil.right);
            float y = Vector3.Dot(dir, pupil.up);

            x *= maxPupilDistance;
            y *= maxPupilDistance;

            pupil.anchoredPosition = new Vector2(x, y);
        }
    }

    IEnumerator RandomBlinking()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval + Random.Range(-0.5f, 0.5f));
            yield return StartCoroutine(Blink());
        }
    }

    IEnumerator Blink()
    {
        float t = 0f;

        // eyelids close
        while (t < blinkDuration / 2f)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(0f, 1f, t / (blinkDuration / 2f));
            foreach (var eyelid in eyelids)
                eyelid.value = value;
            yield return null;
        }

        t = 0f;

        // eyelids open
        while (t < blinkDuration / 2f)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(1f, 0f, t / (blinkDuration / 2f));
            foreach (var eyelid in eyelids)
                eyelid.value = value;
            yield return null;
        }
    }
}
