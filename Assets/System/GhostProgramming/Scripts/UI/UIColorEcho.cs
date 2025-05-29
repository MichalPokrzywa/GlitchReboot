using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIColorEcho : MonoBehaviour
{
    [SerializeField] Color targetColor = Color.red;
    [SerializeField] Image image;
    [SerializeField] float duration = 0.5f;

    public bool active = false;

    Color originalColor;
    Sequence echoSequence;
    bool wantStop = false;
    bool hasCompletedCycle = false;

    void Awake()
    {
        if (image == null)
        {
            Debug.LogError("UIColorEcho: No Image component found!");
            enabled = false;
            return;
        }

        originalColor = image.color;
    }

    void Update()
    {
        if (active && (echoSequence == null || !echoSequence.IsActive()))
        {
            wantStop = false;
            StartEchoAnimationLoop();
        }
        else if (!active && echoSequence != null && echoSequence.IsActive())
        {
            wantStop = true;
            StopEchoAnimation();
        }
    }

    void StartEchoAnimationLoop()
    {
        echoSequence = DOTween.Sequence();
        echoSequence.Append(image.DOColor(targetColor, duration));
        echoSequence.Append(image.DOColor(originalColor, duration));
        echoSequence.OnComplete(() =>
        {
            hasCompletedCycle = true;

            if (wantStop)
            {
                echoSequence.Kill();
                echoSequence = null;
                image.color = originalColor;
            }
            else
            {
                // start next cycle
                StartEchoAnimationLoop();
            }
        });
        echoSequence.Play();
    }

    void StopEchoAnimation()
    {
        if (!hasCompletedCycle)
            return;

        if (echoSequence != null)
        {
            echoSequence.Kill();
            echoSequence = null;
        }
        image.color = originalColor;
    }
}