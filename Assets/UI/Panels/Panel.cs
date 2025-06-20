using System;
using DG.Tweening;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] protected RectTransform contentRect;

    public bool isOpen => panelToggle;
    public GameObject FirstItemToSelect => firstItemToSelect;

    public Action onPanelOpen;
    public Action onPanelClose;

    protected bool panelToggle = false;
    protected bool animationInProgress = false;
    protected GameObject firstItemToSelect;

    Tween tween;

    const float scaleDuration = 0.25f;

    public virtual void TogglePanel()
    {
        if (animationInProgress)
            tween?.Kill();

        panelToggle = !panelToggle;

        if (panelToggle)
            Open();
        else
            Close();
    }

    public virtual void Open()
    {
        if (animationInProgress)
            tween?.Kill();

        panelToggle = true;
        animationInProgress = true;

        tween = contentRect.DOScale(new Vector3(1, 1, 1), scaleDuration).SetUpdate(true).OnComplete(() =>
        {
            animationInProgress = false;
            onPanelOpen?.Invoke();
        });
    }

    public virtual void Close()
    {
        if (animationInProgress)
            tween?.Kill();

        panelToggle = false;
        animationInProgress = true;
        tween = contentRect.DOScale(Vector3.zero, scaleDuration).SetUpdate(true).OnComplete(() =>
        {
            animationInProgress = false;
            onPanelClose?.Invoke();
        });
    }

    public virtual void ResetState()
    {
        panelToggle = false;
        animationInProgress = false;
    }
}
