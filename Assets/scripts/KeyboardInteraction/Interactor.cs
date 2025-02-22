using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform InteractionSource;
    public float InteractionRange;
    private IInteractable lastInteractor;

    void Update()
    {
        Ray r = new Ray(InteractionSource.position, InteractionSource.forward);
        if (Physics.Raycast(r, out RaycastHit hit, InteractionRange))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactObj.Interact();
                }

                if (!interactObj.HasShownUI)
                {
                    interactObj.ShowUI();
                    lastInteractor = interactObj;
                }
            }
            else if (lastInteractor != null)
            {
                lastInteractor.HideUI();
                lastInteractor = null; // Resetujemy
            }
        }
    }
}

interface IInteractable
{
    public void Interact();
    public void ShowUI();
    public void HideUI();
    bool HasShownUI { get; set; }
}
