using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform InteractionSource;
    public float InteractionRange = 10;
    public Transform holdPoint;

    private IInteractable lastInteractor;
    private PickUpObjectInteraction heldObject;

    void Update()
    {
        Ray r = new Ray(InteractionSource.position, InteractionSource.forward);
        if (Physics.Raycast(r, out RaycastHit hit, InteractionRange) &&
            hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (interactObj is PickUpObjectInteraction pickup)
                {
                    if (heldObject != null && heldObject != pickup)
                        heldObject.DropMe();

                    if (!pickup.IsPickedUp)
                    {
                        pickup.PickMeUp(holdPoint);
                        heldObject = pickup;
                        interactObj.HideUI(); // Hide UI when picked up
                    }
                    else
                    {
                        pickup.DropMe();
                        heldObject = null;
                    }
                }
                else
                {
                    interactObj.Interact();
                }
            }

            if (!interactObj.HasShownUI && heldObject == null)
            {
                interactObj.ShowUI();
                lastInteractor = interactObj;
            }
        }
        else if (lastInteractor != null)
        {
            lastInteractor.HideUI();
            lastInteractor = null;
        }
    }
}

interface IInteractable
{
    bool HasShownUI { get; set; }
    public void Interact();
    public void ShowUI();
    public void HideUI();
}