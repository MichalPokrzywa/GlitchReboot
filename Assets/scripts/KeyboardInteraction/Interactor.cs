using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] Transform InteractionSource;
    [SerializeField] float InteractionRange = 10;    // EntityTooltipInteraction is always shown, regardless of this
    [SerializeField] float holdDistance = 2f;        // default distance to hold objects
    [SerializeField] float holdSmoothSpeed = 10f;    // how quickly the holdPoint moves
    [SerializeField] Transform holdPoint;
    [SerializeField] Transform handPoint;
    [SerializeField] FirstPersonController player;

    public bool canInteract = true;
    public Animator animator;

    IInteractable lastInteractor;
    PickUpObjectInteraction heldObject;

    void Update()
    {
        //hold point update
        Vector3 desiredHoldPos = InteractionSource.position + InteractionSource.forward * holdDistance;
        if (Physics.Raycast(InteractionSource.position, InteractionSource.forward, out RaycastHit obsHit, holdDistance))
        {
            // Move just in front of the obstacle (you can subtract a small epsilon if you like)
            desiredHoldPos = InteractionSource.position +
                             InteractionSource.forward * (obsHit.distance * 0.5f);
        }
        // smooth-lerp your holdPoint there
        holdPoint.position = Vector3.Lerp(
            holdPoint.position,
            desiredHoldPos,
            Time.deltaTime * holdSmoothSpeed
        );
        //Debug.Log($"{holdPoint.position}, {handPoint.position}");

        if (!canInteract)
            return;

        HandleInteraction();
    }

    void HandleInteraction()
    {
        Ray r = new Ray(InteractionSource.position, InteractionSource.forward);

        if (heldObject is PickUpObjectInteraction pickupHeld)
        {
            HandleHeldObject(pickupHeld);
            return;
        }

        int layerToIgnore = LayerMask.NameToLayer("Ignore Raycast");
        int layerMask = ~(1 << layerToIgnore);

        if (!Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            ClearLastInteractor();
            return;
        }

        if (!hit.collider.TryGetComponent(out IInteractable interactObj))
        {
            ClearLastInteractor();
            return;
        }

        HandleHoverUI(interactObj, hit);
        HandleInteractKey(interactObj, hit);
    }

    void HandleHeldObject(PickUpObjectInteraction pickup)
    {
        bool canPickup = pickup.IsPickedUp && pickup.inhand && animator.GetBool("CanPickup");

        if (InputManager.Instance.IsInteractPressed() && canPickup)
        {
            pickup.DropInFront();
            heldObject = null;
        }
        else if (InputManager.Instance.IsFirePressed() && canPickup)
        {
            pickup.Throw();
            heldObject = null;
        }
    }

    void HandleInteractKey(IInteractable interactObj, RaycastHit hit)
    {
        if (!InputManager.Instance.IsInteractPressed() || hit.distance > InteractionRange)
            return;

        bool canPickup = heldObject == null && animator.GetBool("CanPickup") &&
                         MechanicsManager.Instance.IsEnabled(MechanicType.PickUp);

        // logic for pickup object
        if (interactObj is PickUpObjectInteraction pickup && canPickup)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerWithObject");
            pickup.MoveToHand(handPoint, holdPoint, this);
            PanelManager.Instance.ShowTipsOnce(TipsPanel.eTipType.DiceDrop, TipsPanel.eTipType.DiceThrow);
            heldObject = pickup;
            interactObj.HideUI();
        }
        // logic for the rest of the interactables
        else
        {
            interactObj.Interact();
        }
    }

    // ugly garbage, please fix me some day
    void HandleHoverUI(IInteractable interactObj, RaycastHit hit)
    {
        bool canPickup = animator.GetBool("CanPickup") && MechanicsManager.Instance.IsEnabled(MechanicType.PickUp);
        ClearLastInteractor();

        switch (interactObj)
        {
            case EntityTooltipInteraction entity when player.isZoomed:
                entity.ShowUI();
                lastInteractor = entity;
                break;

            case EntityTooltipInteraction when !player.isZoomed:
                ClearLastInteractor();
                break;

            case PickUpObjectInteraction pickup when hit.distance <= InteractionRange && heldObject == null && canPickup:
                pickup.ShowUI();
                lastInteractor = pickup;
                break;

            case PickUpObjectInteraction pickup when pickup.EntityTooltipInteraction != null && player.isZoomed:
                pickup.EntityTooltipInteraction.ShowUI();
                lastInteractor = pickup.EntityTooltipInteraction;
                break;

            default:
                if (interactObj is not EntityTooltipInteraction
                    && interactObj is not PickUpObjectInteraction
                    && hit.distance <= InteractionRange)
                {
                    interactObj.ShowUI();
                    lastInteractor = interactObj;
                }
                break;
        }
    }

    void ClearLastInteractor()
    {
        lastInteractor?.HideUI();
        lastInteractor = null;
    }

    public void HideLastUI()
    {
        ClearLastInteractor();
    }

    // called by the pickup when it actually drops
    public void NotifyDropped(PickUpObjectInteraction obj)
    {
        if (heldObject == obj)
            heldObject = null;
    }

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }

}

interface IInteractable
{
    bool HasShownUI { get; set; }
    public void Interact();
    public void ShowUI();
    public void HideUI();
}