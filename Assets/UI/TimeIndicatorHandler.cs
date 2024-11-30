using UnityEngine;
using UnityEngine.UI;

public class TimeIndicatorHandler : MonoBehaviour
{
    public Image cooldownCircle; // Okrąg używany jako wskaźnik odliczania
    public Image readyDot;       // Ikona "gotowości", gdy odliczanie nie jest aktywne
    public Image redDot;         // Ikona "nagrywania", gdy proces jest aktywny
    public Color color;          // Kolor ikon i okręgu

    private float countdownTime; // Łączny czas odliczania
    private float elapsedTime;   // Upływający czas
    private bool isCounting = false;  // Czy odliczanie jest w toku
    private bool isRecording = false; // Czy nagrywanie jest w toku
    private bool isAnimatingReadyDot = false;

    private void Awake()
    {
        if (cooldownCircle != null)
            cooldownCircle.material.color = color;
        if (readyDot != null)
            readyDot.material.color = color;

        UpdateIcons();
    }

    private void Update()
    {
        // Test: Naciśnij Spację, aby rozpocząć odliczanie
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCountdown(3.0f);
        }

        if (isCounting)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(elapsedTime / countdownTime);
            if (cooldownCircle != null)
                cooldownCircle.material.SetFloat("_Slider", fillAmount);

            if (elapsedTime >= countdownTime)
            {
                isCounting = false;
                AnimateReadyDot(false); // Cofnij animację
                UpdateIcons();
            }
        }
    }

    public void StartCountdown(float time)
    {
        AnimateReadyDot(true);

        countdownTime = time;
        elapsedTime = 0f;
        isCounting = true;

        UpdateIcons();
        AnimateReadyDot(true); // Rozpocznij animację w przód

        if (cooldownCircle != null)
            cooldownCircle.material.SetFloat("_Slider", 0);
    }

    public void StopCountdown()
    {
        isCounting = false;
        elapsedTime = 0f;

        if (cooldownCircle != null)
            cooldownCircle.material.SetFloat("_Slider", 0);

        AnimateReadyDot(false);
        UpdateIcons();
    }

    public void StartRecording()
    {
        isRecording = true;
        UpdateIcons();

        if (cooldownCircle != null)
            cooldownCircle.enabled = false;
        if (readyDot != null)
            readyDot.enabled = false;
    }

    public void StopRecording()
    {
        isRecording = false;
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        if (cooldownCircle != null)
            cooldownCircle.enabled = isCounting || isRecording;

        if (readyDot != null)
            readyDot.enabled = !isRecording;

        if (redDot != null)
            redDot.enabled = isRecording;
    }

    private void AnimateReadyDot(bool forward)
    {
        if (readyDot == null || isAnimatingReadyDot) return;

        isAnimatingReadyDot = true;
        StartCoroutine(AnimateMaterial(readyDot.material, forward ? 0 : 1, forward ? 1 : 0, 0.25f));
    }

    private System.Collections.IEnumerator AnimateMaterial(Material material, float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            material.SetFloat("_Slider", Mathf.Lerp(from, to, t));
            yield return null;
        }
        material.SetFloat("_Slider", to);
        isAnimatingReadyDot = false;
    }
}
