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

    private void Awake()
    {
        // Ustawienie koloru dla elementów UI
        if (cooldownCircle != null)
        {
            cooldownCircle.color = color;
            // Debug.Log("[TimeIndicatorHandler] Kolor ustawiony dla cooldownCircle.");
        }
        if (readyDot != null)
        {
            readyDot.color = color;
            // Debug.Log("[TimeIndicatorHandler] Kolor ustawiony dla readyDot.");
        }

        UpdateIcons();
    }

    private void Update()
    {
        // Test: Naciśnij Spację, aby rozpocząć 10-sekundowe odliczanie
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartCountdown(10.0f);
        // }

        if (isCounting)
        {
            // Zwiększ upływający czas
            elapsedTime += Time.deltaTime;

            // Oblicz, jaki procent czasu minął i zaktualizuj wypełnienie okręgu
            float fillAmount = Mathf.Clamp01(1 - (elapsedTime / countdownTime));
            if (cooldownCircle != null)
                cooldownCircle.fillAmount = fillAmount;

            // Debugowanie progresu
            // Debug.Log($"[TimeIndicatorHandler] Odliczanie w toku: {elapsedTime}/{countdownTime} sekund.");

            // Zatrzymaj odliczanie, jeśli czas się skończył
            if (elapsedTime >= countdownTime)
            {
                isCounting = false;
                // Debug.Log("[TimeIndicatorHandler] Odliczanie zakończone.");
                UpdateIcons();

                // Ustaw okrąg jako pusty
                if (cooldownCircle != null)
                    cooldownCircle.fillAmount = 0;
            }
        }
    }

    /// <summary>
    /// Rozpoczyna odliczanie.
    /// </summary>
    /// <param name="time">Czas w sekundach, przez jaki okrąg będzie się zapełniać.</param>
    public void StartCountdown(float time)
    {
        countdownTime = time;
        elapsedTime = 0f;
        isCounting = true;
        Debug.Log($"[TimeIndicatorHandler] Rozpoczęto odliczanie: {countdownTime} sekund.");
        UpdateIcons();

        // Rozpocznij od pełnego okręgu
        if (cooldownCircle != null)
            cooldownCircle.fillAmount = 1;
    }

    /// <summary>
    /// Zatrzymuje odliczanie i resetuje wskaźnik.
    /// </summary>
    public void StopCountdown()
    {
        isCounting = false;
        elapsedTime = 0f;

        Debug.Log("[TimeIndicatorHandler] Odliczanie zatrzymane.");

        if (cooldownCircle != null)
            cooldownCircle.fillAmount = 0;

        UpdateIcons();
    }

    /// <summary>
    /// Rozpoczyna nagrywanie.
    /// </summary>
    public void StartRecording()
    {
        isRecording = true;
        Debug.Log("[TimeIndicatorHandler] Nagrywanie rozpoczęte.");
        UpdateIcons();
    }

    /// <summary>
    /// Zatrzymuje nagrywanie.
    /// </summary>
    public void StopRecording()
    {
        isRecording = false;
        Debug.Log("[TimeIndicatorHandler] Nagrywanie zatrzymane.");
        UpdateIcons();
    }

    /// <summary>
    /// Aktualizuje widoczność ikon w zależności od stanu odliczania i nagrywania.
    /// </summary>
    private void UpdateIcons()
    {
        // Okrąg widoczny tylko podczas odliczania
        if (cooldownCircle != null)
            cooldownCircle.enabled = isCounting;

        // Ikona gotowości widoczna, gdy odliczanie jest nieaktywne
        if (readyDot != null)
            readyDot.enabled = !isCounting && !isRecording;

        // Ikona nagrywania widoczna, gdy nagrywanie jest aktywne
        if (redDot != null)
            redDot.enabled = isRecording;

        Debug.Log($"[TimeIndicatorHandler] Ikony zaktualizowane. isCounting: {isCounting}, isRecording: {isRecording}");
    }
}
