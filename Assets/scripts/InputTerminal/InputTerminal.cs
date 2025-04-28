using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputTerminal : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button closeButton;

    bool isOpen = true;

    const float animDuration = 0.5f;
    const string defaultCode =
        "function GhostControl()" +
        "   Ghost1:PickUp(Cube1)" +
        "end";

    void Start()
    {
        closeButton.onClick.AddListener(CloseTerminal);
        inputField.onValueChanged.AddListener(OnInputFieldChanged);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isOpen)
                CloseTerminal();
            else
                OpenTerminal();
        }
    }

    void OnDestroy()
    {
        closeButton.onClick.RemoveListener(CloseTerminal);
        inputField.onValueChanged.RemoveListener(OnInputFieldChanged);
    }

    void OnInputFieldChanged(string text)
    {

    }

    void OpenTerminal()
    {
        isOpen = true;
        rectTransform.DOScaleX(1, animDuration).OnComplete(() =>
        {
            gameObject.SetActive(true);
        });
    }

    void CloseTerminal()
    {
        isOpen = false;
        rectTransform.DOScaleX(0, animDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

}
