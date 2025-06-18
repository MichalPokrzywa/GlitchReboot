using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform InteractionSource;
    public float InteractionRange = 10;
    public float holdDistance = 2f;        // default distance to hold objects
    public float holdSmoothSpeed = 10f;    // how quickly the holdPoint moves
    public Transform holdPoint;
    public Transform handPoint;
    public bool canInteract = true;
    public Animator animator;
    public FirstPersonController player;
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
                             InteractionSource.forward * (obsHit.distance * 0.5f);
        }
        // smooth-lerp your holdPoint there
        holdPoint.position = Vector3.Lerp(
            holdPoint.position,
            desiredHoldPos,
            Time.deltaTime * holdSmoothSpeed
        );
        //Debug.Log($"{holdPoint.position}, {handPoint.position}");

        if(!canInteract)
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

        if (!Physics.Raycast(r, out RaycastHit hit))
        {
            ClearLastInteractor();
            return;
        }

        if (!hit.collider.TryGetComponent(out IInteractable interactObj))
        {
            ClearLastInteractor();
            return;
        }

        HandleInteractKey(interactObj, hit);
        HandleHoverUI(interactObj, hit);
    }

    void HandleHeldObject(PickUpObjectInteraction pickup)
    {
        bool canPickup = pickup.IsPickedUp && pickup.inhand && animator.GetBool("CanPickup");

        if (Input.GetKeyDown(KeyCode.E) && canPickup)
        {
            pickup.DropInFront();
            heldObject = null;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && canPickup)
        {
            pickup.Throw();
            heldObject = null;
        }
    }

    void HandleInteractKey(IInteractable interactObj, RaycastHit hit)
    {
        if (!Input.GetKeyDown(KeyCode.E) || hit.distance > InteractionRange)
            return;

        // logic for pickup object
        if (interactObj is PickUpObjectInteraction pickup
            && pickup.IsDropped && heldObject == null && animator.GetBool("CanPickup"))
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerWithObject");
            pickup.MoveToHand(handPoint, holdPoint, this);
            PanelManager.Instance.ShowTipOnce(TipsPanel.eTipType.DiceThrow);
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
        bool canPickup = animator.GetBool("CanPickup");

        // logic for entity tooltip
        if (interactObj is EntityTooltipInteraction entity)
        {
            // if is zoomed, show entity tooltip
            if (player.isZoomed)
            {
                entity.ShowUI();
                lastInteractor = entity;
            }
            // if not zoomed, make sure that tooltip is hidden
            else if (entity.HasShownUI)
            {
                ClearLastInteractor();
            }
        }
        // logic for the rest of the interactables
        else if (heldObject == null && canPickup)
        {
            // default interactable
            if (hit.distance <= InteractionRange)
            {
                interactObj.ShowUI();
                lastInteractor = interactObj;
            }
            // pickup interactable covering entity tooltip
            else if (interactObj is PickUpObjectInteraction pickup && player.isZoomed)
            {
                pickup.EntityTooltipInteraction.ShowUI();
                lastInteractor = pickup.EntityTooltipInteraction;
            }
            // if not zoomed, make sure that tooltip is hidden
            else
            {
                ClearLastInteractor();
            }
        }
    }

    void ClearLastInteractor()
    {
        lastInteractor?.HideUI();
        lastInteractor = null;
    }

    public void HideLastUI()
    {
        lastInteractor?.HideUI();
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