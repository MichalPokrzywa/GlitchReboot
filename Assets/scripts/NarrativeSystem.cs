using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrativeSystem : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();

    static NarrativeSystem _instance;

    Coroutine fadeCoroutine;
    Color startColor;

    public static NarrativeSystem instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<NarrativeSystem>();
                if (_instance == null)
                {
                    GameObject singletonGO = new GameObject("NarrativeSystem (Singleton)");
                    _instance = singletonGO.AddComponent<NarrativeSystem>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        startColor = textDisplay.color;
    }

    public void SetText(string text, float displayTime = 3f, Color? color = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        textDisplay.color = color != null ? color.Value : startColor;
        textDisplay.text = text;
        SetAlpha(1f);
        fadeCoroutine = StartCoroutine(FadeOutAfterDelay(displayTime));
    }

    public void Play(int key)
    {
        AudioClip voice = audioClips[key - 1];

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = voice;
        audioSource.Play();
    }

    void SetAlpha(float alpha)
    {
        Color color = textDisplay.color;
        color.a = alpha;
        textDisplay.color = color;
    }

    IEnumerator FadeOutAfterDelay(float displayTime = 3f)
    {
        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        textDisplay.text = string.Empty;
        fadeCoroutine = null;
    }
}
