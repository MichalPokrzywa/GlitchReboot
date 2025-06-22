using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NarrativeSystem : Singleton<NarrativeSystem>
{
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] private float typeSpeed = 0.05f;

    Coroutine runningCoroutine;
    AudioSource audioSource;
    Color startColor;

    const float constDisplayTime = 3f;

    public bool IsPlaying => audioSource.isPlaying;

    void Start()
    {
        startColor = textDisplay.color;
        audioSource = DependencyManager.audioManager.soundsAudioSource;
    }

    public void SetText(string text, float additionalDisplayTime = 0f, Color? color = null)
    {
        // Stop any in-progress type/fade
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        // Reset color & alpha
        textDisplay.color = color ?? startColor;
        SetAlpha(1f);

        // Start the new typewriter+fade coroutine
        runningCoroutine = StartCoroutine(TypeAndFade(text, typeSpeed, constDisplayTime + additionalDisplayTime));
    }

    public void Play(Scene scene, int id)
    {
        var audio = DependencyManager.audioManager.audioStorage.GetSpiderVoiceOver(scene, id - 1);

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = audio;
        audioSource.Play();
    }

    void SetAlpha(float alpha)
    {
        Color color = textDisplay.color;
        color.a = alpha;
        textDisplay.color = color;
    }

    private IEnumerator TypeAndFade(string fullText, float charInterval, float displayTime)
    {
        // Typewriter effect
        textDisplay.text = "";
        StringBuilder displayed = new StringBuilder();
        int i = 0;
        while (i < fullText.Length)
        {
            if (fullText[i] == '<')
            {
                // Detected start of a rich-text tag. Find the closing '>'.
                int closeIndex = fullText.IndexOf('>', i);
                if (closeIndex == -1)
                {
                    // Malformed tag: no closing '>'. Just append the rest and break.
                    displayed.Append(fullText.Substring(i));
                    i = fullText.Length;
                }
                else
                {
                    // Append the entire tag at once
                    string tag = fullText.Substring(i, closeIndex - i + 1);
                    displayed.Append(tag);
                    i = closeIndex + 1;
                }
                // No delay for tags
            }
            else
            {
                // Append a single visible character
                displayed.Append(fullText[i]);
                i++;
                // Update text and wait
                textDisplay.text = displayed.ToString();
                yield return new WaitForSeconds(charInterval);
            }

            // After appending tag or character, always update the displayed text
            textDisplay.text = displayed.ToString();
        }
        // Wait before fading
        yield return new WaitForSeconds(displayTime);

        // Fade out
        float elapsed = 0f;
        Color original = textDisplay.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlpha(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        // Ensure fully hidden
        SetAlpha(0f);
        textDisplay.text = "";
        runningCoroutine = null;
    }

    public void ResetNarrative()
    {
        StopAllCoroutines();
        SetAlpha(1f);
        textDisplay.text = string.Empty;
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.time = 0f;
        }
    }
}
