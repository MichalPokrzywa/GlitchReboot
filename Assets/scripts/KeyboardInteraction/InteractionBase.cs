using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static TipsPanel;

public class InteractionBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected GameObject UIHoverObject;
    [SerializeField] protected TextMeshProUGUI UIHoverText;
    [SerializeField] protected string actionSuffix;
    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected float floatDistanceX = 100f; // Maksymalna wysokość pływania
    [SerializeField] protected float floatDistanceY = 100f;

    public bool HasShownUI { get; set; }

    protected string TooltipText = string.Empty;

    Vector3 originalPosition;
    string displayedText;

    protected virtual void Start()
    {
        Init();
    }

    void Reset()
    {
        Init();
    }

    public virtual void Update()
    {
        if (HasShownUI)
        {
            AnimateUI();
        }
    }

    void Init()
    {
        // 1) Find the GameObject (active or inactive)
        if (UIHoverObject == null)
        {
            UIHoverObject = Resources
                .FindObjectsOfTypeAll<GameObject>()
                .FirstOrDefault(go => go.name == "UI Hover Text");
            if (UIHoverObject == null)
                Debug.LogError("Could not find UI Hover Text in loaded resources!");
        }

        // 2) Grab the TMP component if it wasn’t already set
        if (UIHoverObject != null && UIHoverText == null)
        {
            UIHoverText = UIHoverObject.GetComponent<TextMeshProUGUI>();
            if (UIHoverText == null)
                Debug.LogError("UI Hover Text object has no TextMeshProUGUI!");
            UIHoverObject.SetActive(false);
        }

        // 3) Cache main camera
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public virtual void Interact()
    {
        Debug.Log("InteractionNotImplemented");
    }

    public void ShowUI()
    {
        //Debug.Log("Show UI");
        UIHoverObject.SetActive(true);

        if (string.IsNullOrEmpty(TooltipText))
        {
            var currentControls = InputManager.Instance.CurrentControls.Player;
            var binding = InputManager.Instance.GetBinding(currentControls.Interact);
            displayedText = $"{binding} {actionSuffix}";
        }
        else
        {
            displayedText = $"{TooltipText} {actionSuffix}";
        }

        UIHoverText.text = displayedText;
        HasShownUI = true;
    }

    public void HideUI()
    {
       // Debug.Log("Hide UI");
        UIHoverObject.SetActive(false);
        HasShownUI = false;
        displayedText = string.Empty;
    }

    public void AnimateUI()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 screenPos = mainCamera.ViewportToScreenPoint(viewportPos);

        Vector3 offset = new Vector3(floatDistanceX * -1, floatDistanceY, 0);
        Vector3 targetPosition = screenPos + offset;

        float textWidth = UIHoverText.preferredWidth;
        float textHeight = UIHoverText.preferredHeight;

        //Debug.Log($"{name} - {textWidth}");
        //Debug.Log($"{name} - {textHeight}");

        float margin = 10f;
        targetPosition.x = Mathf.Clamp(targetPosition.x, margin, Screen.width - textWidth - margin);
        targetPosition.y = Mathf.Clamp(targetPosition.y, margin + textHeight, Screen.height - margin);

        UIHoverText.rectTransform.position = Vector3.Lerp(
                UIHoverText.rectTransform.position,
                targetPosition,
                Time.deltaTime * 5f
            );
    }
}
