using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform InteractionSource;
    public float InteractionRange = 10;
    public float holdDistance = 2f;        // default distance to hold objects
    public float holdSmoothSpeed = 10f;    // how quickly the holdPoint moves
    public Transform holdPoint;
    public Transform handPoint;

    private IInteractable lastInteractor;
    private PickUpObjectInteraction heldObject;

    void Update()
    {
        //hold point update
        Vector3 desiredHoldPos = InteractionSource.position + InteractionSource.forward * holdDistance;
        if (Physics.Raycast(InteractionSource.position, InteractionSource.forward, out RaycastHit obsHit, holdDistance))
        {
            // Move just in front of the obstacle (you can subtract a small epsilon if you like)
            desiredHoldPos = InteractionSource.position +
                             InteractionSource.forward * (obsHit.distance * 0.9f);
        }
        // smooth-lerp your holdPoint there
        holdPoint.position = Vector3.Lerp(
            holdPoint.position,
            desiredHoldPos,
            Time.deltaTime * holdSmoothSpeed
        );
        Ray r = new Ray(InteractionSource.position, InteractionSource.forward);


        if (heldObject != null)
        {
            if (heldObject is PickUpObjectInteraction pickup)
            {
                if (Input.GetKeyDown(KeyCode.E) && pickup.IsPickedUp)
                {
                    pickup.DropInFront();
                    heldObject = null;

                }
                else if (Input.GetKeyDown(KeyCode.Mouse0) && pickup.IsPickedUp)
                {
                    pickup.Throw();
                    heldObject = null;
                }
            }
        }
        if (Physics.Raycast(r, out RaycastHit hit, InteractionRange) &&
            hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (interactObj is PickUpObjectInteraction pickup)
                {
                    if (!pickup.IsPickedUp && heldObject == null && heldObject != pickup)
                    {
                        // pass yourself into the pickup
                        pickup.MoveToHand(handPoint, holdPoint, this);
                        heldObject = pickup;
                        interactObj.HideUI(); // Hide UI when picked up
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

    // called by the pickup when it actually drops
    public void NotifyDropped(PickUpObjectInteraction obj)
    {
        if (heldObject == obj)
            heldObject = null;
    }
}

interface IInteractable
{
    bool HasShownUI { get; set; }
    public void Interact();
    public void ShowUI();
    public void HideUI();
}