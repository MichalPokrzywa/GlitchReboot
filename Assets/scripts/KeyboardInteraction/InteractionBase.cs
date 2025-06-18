using System.Linq;
using TMPro;
using UnityEngine;

public class InteractionBase : MonoBehaviour, IInteractable
{
    public GameObject UIHoverObject;
    public TextMeshProUGUI UIHoverText;
    protected string TooltipText = "[E] Use";
    public float floatDistanceX = 100f; // Maksymalna wysokość pływania
    public float floatDistanceY = 100f;
    private Vector3 originalPosition;
    public Camera mainCamera;
    public bool HasShownUI { get; set; }

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
        UIHoverText.text = TooltipText;
        HasShownUI = true;
    }

    public void HideUI()
    {
       // Debug.Log("Hide UI");
        UIHoverObject.SetActive(false);
        HasShownUI = false;
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
