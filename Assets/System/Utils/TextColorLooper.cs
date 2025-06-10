using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class TextColorLooper : MonoBehaviour
{

    [Header("All TMP_Text elements to animate")]
    [Tooltip("Drag in every TMP_Text field you want to loop the color on.")]
    [HideInInspector]
    public List<TMP_Text> textList;

    [Header("Color‐cycling Settings")]
    [Tooltip("The sequence of colors to loop through (e.g. red, green, blue).")]
    public Color[] cycleColors = new Color[] { Color.red, Color.green, Color.blue };

    [Tooltip("Seconds to interpolate from one color to the next.")]
    public float durationPerColor = 0.5f;

    private Coroutine loopCoroutine = null;
    private bool isLooping = false;

    public void StartLoop()
    {
        if (isLooping) return;
        isLooping = true;
        loopCoroutine = StartCoroutine(TextColorLoopCoroutine());
    }

    public void StopLoop()
    {
        if (!isLooping) return;
        isLooping = false;

        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }

        // Immediately set all texts back to white
        foreach (var tmp in textList)
        {
            tmp.color = Color.white;
        }
    }

    private IEnumerator TextColorLoopCoroutine()
    {
        if (cycleColors == null || cycleColors.Length == 0)
        {
            yield break;
        }

        int idx = 0;
        while (isLooping)
        {
            Color startColor = cycleColors[idx];
            Color endColor = cycleColors[(idx + 1) % cycleColors.Length];
            float t = 0f;

            while (t < durationPerColor && isLooping)
            {
                t += Time.deltaTime;
                float lerp = Mathf.Clamp01(t / durationPerColor);
                Color current = Color.Lerp(startColor, endColor, lerp);

                foreach (var tmp in textList)
                {
                    tmp.color = current;
                }

                yield return null;
            }

            idx = (idx + 1) % cycleColors.Length;
        }

        loopCoroutine = null;
    }
}
