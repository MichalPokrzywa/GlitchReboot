using System.Linq;
using TMPro;
using UnityEngine;

public class InteractionBase : MonoBehaviour, IInteractable
{
    public GameObject UIHoverObject;
    public TextMeshProUGUI UIHoverText;
    protected string TooltipText = "[E] Użyj";
    public float floatDistanceX = 100f; // Maksymalna wysokość pływania
    public float floatDistanceY = 100f;
    private Vector3 originalPosition;
    public Camera mainCamera;
    public bool HasShownUI { get; set; }

    protected virtual void Awake()
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
        // Przekształcenie pozycji obiektu na ekran
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        // Zyskujemy szerokość tekstu (można także użyć preferredWidth z TextMeshProUGUI)
        float textWidth = UIHoverText.preferredWidth;
        Vector3 targetPosition = Vector3.zero;

        targetPosition = screenPos + new Vector3((floatDistanceX  * -1), floatDistanceY, 0);

        UIHoverText.rectTransform.position = Vector3.Lerp(UIHoverText.rectTransform.position, targetPosition, Time.deltaTime * 5f);
    }
}
