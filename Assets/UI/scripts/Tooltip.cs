using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] FirstPersonController target;
    [SerializeField] TMP_Text textComponent;
    [SerializeField] bool requireZoom = true;

    Vector3 visibleScale = Vector3.one;
    Vector3 hiddenScale = Vector3.zero;

    bool isActive = false;

    void Awake()
    {
        if (target == null)
        {
            target = FindFirstObjectByType<FirstPersonController>();
            if (target == null)
                Debug.LogWarning("Tooltip: No FirstPersonController found in the scene.");
        }

        HideTooltip();
    }

    void LateUpdate()
    {
        if (!isActive || target == null) return;

        if (!requireZoom || target.isZoomed)
        {
            ShowTooltip();
            FaceTarget();
        }
        else
        {
            HideTooltip();
        }
    }

    public void Activate(string text)
    {
        textComponent.text = text;
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (transform.localScale != visibleScale)
            transform.localScale = visibleScale;
    }

    private void HideTooltip()
    {
        if (transform.localScale != hiddenScale)
            transform.localScale = hiddenScale;
    }

    void FaceTarget()
    {
        transform.LookAt(target.transform);
        transform.Rotate(0f, 180f, 0f);
    }
}
