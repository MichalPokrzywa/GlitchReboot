using TMPro;
using UnityEngine;

public class InteractionBase : MonoBehaviour, IInteractable
{
    public GameObject UIHoverObject;
    public TextMeshProUGUI UIHoverText;
    private string TooltipText = "[E] Użyj";
    public float floatDistanceX = 100f; // Maksymalna wysokość pływania
    public float floatDistanceY = 100f;
    private Vector3 originalPosition;
    public Camera mainCamera;
    public bool HasShownUI { get; set; }

    void Reset()
    {
        if (UIHoverObject == null)
        {
            UIHoverObject = GameObject.Find("UI Hover Text");
            UIHoverText = UIHoverObject.GetComponent<TextMeshProUGUI>();
        }
        if(mainCamera == null)
            mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        if (HasShownUI)
        {
            AnimateUI();
        }
    }

    public virtual void Interact()
    {
        Debug.Log("InteractionNotImplemented");
    }

    public void ShowUI()
    {
        Debug.Log("Show UI");
        UIHoverObject.SetActive(true);
        UIHoverText.text = TooltipText;
        HasShownUI = true;
    }

    public void HideUI()
    {
        Debug.Log("Hide UI");
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
