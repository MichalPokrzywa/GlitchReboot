using System;
using TMPro;
using UnityEngine;

public class InteractionTest : MonoBehaviour, IInteractable
{
    public GameObject UIHoverObject;
    public TextMeshProUGUI UIHoverText;
    private string TooltipText = "[E] Użyj";
    public bool HasShownUI { get; set; }
    public float floatSpeed = 1f;  // Szybkość pływania
    public float floatDistanceX = 100f; // Maksymalna wysokość pływania
    public float floatDistanceY = 30f;
    private Vector3 originalPosition;
    public Camera mainCamera;
    public GameObject objectWithInteractiveMaterial;

    public void Start()
    {
        UIHoverObject = GameObject.Find("UI Hover Text");
        UIHoverText = UIHoverObject.GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        if (HasShownUI)
        {
            AnimateUI();
        }
    }
    
    public void Interact()
    {
        Debug.Log("Interaction");
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
    
    public void ShowUI()
    {
        Debug.Log("Show UI");
        UIHoverObject.SetActive(true);
        UIHoverText.text = TooltipText;
        HasShownUI = true;
        // TODO To chcemy używać rzadko nie za każdym razem i z dopracowanym efektem
        // if(objectWithInteractiveMaterial != null) objectWithInteractiveMaterial.GetComponent<Renderer>().material.SetFloat("_activate", 1f);
        // StartCoroutine(EndAnimationAfterDelay(0.125f));
    }

    public void HideUI()
    {
        Debug.Log("Hide UI");
        UIHoverObject.SetActive(false);
        HasShownUI = false;
    }
    
    private System.Collections.IEnumerator EndAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(objectWithInteractiveMaterial != null) objectWithInteractiveMaterial.GetComponent<Renderer>().material.SetFloat("_activate", 0f);
    }
}
