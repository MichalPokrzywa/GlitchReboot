using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    public float glitchFrequency = 0.2f; // Jak często występuje glitch
    public float glitchDuration = 0.05f; // Jak długo trwa glitch
    public float positionIntensity = 0.1f; // Intensywność przesunięcia pozycji
    public float rotationIntensity = 5f; // Intensywność rotacji

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isGlitching = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        InvokeRepeating("StartGlitch", glitchFrequency, glitchFrequency);
    }

    void StartGlitch()
    {
        if (!isGlitching)
        {
            isGlitching = true;
            StartCoroutine(Glitch());
        }
    }

    System.Collections.IEnumerator Glitch()
    {
        transform.localPosition = originalPosition + new Vector3(
            Random.Range(-positionIntensity, positionIntensity),
            Random.Range(-positionIntensity, positionIntensity),
            Random.Range(-positionIntensity, positionIntensity)
        );

        transform.localRotation = Quaternion.Euler(
            originalRotation.eulerAngles + new Vector3(
                Random.Range(-rotationIntensity, rotationIntensity),
                Random.Range(-rotationIntensity, rotationIntensity),
                Random.Range(-rotationIntensity, rotationIntensity)
            )
        );

        yield return new WaitForSeconds(glitchDuration);

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        isGlitching = false;
    }
}