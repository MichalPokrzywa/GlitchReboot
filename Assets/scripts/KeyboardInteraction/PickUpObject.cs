using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PickUpObjectInteraction : InteractionBase
{
   private Transform holdPoint;
   private Transform handPoint;
   private Rigidbody rb;
   public bool IsPickedUp { get; private set; }

   [Header("Rotation Snap")]
   [Tooltip("Speed (deg/sec) at which to rotate toward the snapped orientation.")]
   public float rotationSpeed = 20f;
   public float moveSpeed = 20f;
   private Interactor ownerInteractor;
   public bool onTarget = false;
   public bool isAnimationPlaying = false;
   public bool inhand = false;
   private Transform diceTransform;
   protected override void Awake()
   {
       base.Awake();
       EntityManager.instance.Register(gameObject);
   }

   private void Start()
   {
      rb = GetComponent<Rigidbody>();
      IsPickedUp = false;
   }

   public override void Interact()
   {
      //if (iAmPickedUp)
      //{
      //    DropMe();
      //}
      //else
      //{
      //    PickMeUp();
      //}
   }

   public override void Update()
   {
       if (!IsPickedUp && HasShownUI)
       {
           AnimateUI();
       }
       if (IsPickedUp)
       {
            if (handPoint  != null) 
            {
                if (Vector3.Distance(handPoint.position, transform.position) > 0.2f && !onTarget)
                {
                    transform.position = Vector3.MoveTowards(transform.position, handPoint.position, Time.deltaTime * moveSpeed);
                    rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, handPoint.rotation, rotationSpeed));
                }
                else
                {
                    onTarget = true;
                    
                    transform.position = handPoint.position;
                    //rb.linearVelocity = (handPoint.position - transform.position) * (1 / Time.fixedDeltaTime);
                    rb.MovePosition(Vector3.Lerp(transform.position, handPoint.position, Time.deltaTime * moveSpeed));
                    //rb.MoveRotation(Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, Time.deltaTime * rotationSpeed));
                    transform.localRotation = Quaternion.identity;
                }

                if (onTarget && !inhand)
                {
                    GetComponent<BoxCollider>().enabled = false;
                    transform.SetParent(handPoint);
                    inhand = true;
                    ownerInteractor.animator.SetBool("InHand", inhand);
                    isAnimationPlaying = false;
                }
            }
            else
            {
                rb.linearVelocity = (holdPoint.position - transform.position) * (1 / Time.fixedDeltaTime);
                rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, holdPoint.rotation, rotationSpeed));
            }

       }
   }

   public void MoveToHand(Transform handTransform, Transform dropPoint, Interactor interactor)
   {
       //transform.DOMove(handTransform.position, 0.2f);
       rb.useGravity = false;
       isAnimationPlaying = true;
        transform.DOScale(0.3f, 0.2f).OnComplete(() =>
       {
           rb.linearDamping = 100;
           rb.angularDamping = 10;
           rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
           IsPickedUp = true;
           //transform.SetParent(handTransform);
       });
       ownerInteractor = interactor;
       ownerInteractor.animator.SetTrigger("PickedDice");
       handPoint = handTransform;
       holdPoint = dropPoint;
       onTarget = false;
       inhand = false;
       ownerInteractor.animator.SetBool("InHand",onTarget);
   }

   public void DropInFront()
   {
       StartCoroutine(DropDiceCoroutine());

   }

   private IEnumerator DropDiceCoroutine()
   { 
        isAnimationPlaying = true;
        ownerInteractor.animator.SetTrigger("DropDice");

        yield return new WaitForSeconds(0.3f);

        transform.SetParent(null);
        transform.DOScale(1f, 0.1f);
        transform.DOMove(holdPoint.position, 0.1f);
        transform.localRotation = Quaternion.identity;
        rb.useGravity = true;
        rb.linearDamping = 1;
        rb.angularDamping = 0.05f;
        rb.constraints = RigidbodyConstraints.None;
        handPoint = null;
        holdPoint = null;
        IsPickedUp = false;
        GetComponent<BoxCollider>().enabled = true;
        onTarget = false;
        inhand = false;
        ownerInteractor.animator.SetBool("InHand", onTarget);
        ownerInteractor?.NotifyDropped(this);
        ownerInteractor = null;

        isAnimationPlaying = false;
   }


   public void Throw()
   {
       StartCoroutine(ThrowDiceCoroutine());
   }
   public IEnumerator ThrowDiceCoroutine()
   {
       ownerInteractor.animator.SetTrigger("ThrowDice");
       yield return new WaitForSeconds(0.35f);
       transform.SetParent(null);
       transform.DOScale(1f, 0.1f);

       if (Vector3.Distance(ownerInteractor.transform.position, holdPoint.position) <
           Vector3.Distance(ownerInteractor.transform.position, handPoint.position))
       {
           transform.position = holdPoint.position;
       }
       handPoint = null;
       holdPoint = null;
       rb.useGravity = true;
       rb.linearDamping = 1f;
       rb.angularDamping = 0.05f;
       rb.constraints = RigidbodyConstraints.None;
       IsPickedUp = false;
       GetComponent<BoxCollider>().enabled = true;
       onTarget = false;
       inhand = false;
       ownerInteractor.animator.SetBool("InHand", onTarget);
       Vector3 dir = mainCamera.transform.forward;
       rb.AddForce(dir * 30f, ForceMode.Impulse);
       ownerInteractor?.NotifyDropped(this);
       ownerInteractor = null;
       isAnimationPlaying = false;
    }


   public void PickMeUp(Transform point, Interactor interactor)
   {
       holdPoint = point;
       ownerInteractor = interactor;

       rb.useGravity = false;
       rb.linearDamping = 10;
       onTarget = false;
       rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
       IsPickedUp = true;
   }

   public bool DropMe()
   {
       if(IsPickedUp)
           return true;
       // reset physics
       rb.useGravity = true;
       rb.linearDamping = 1;
       rb.constraints = RigidbodyConstraints.None;
       IsPickedUp = false;
       onTarget = false;
        // let the interactor know to clear its heldObject
        ownerInteractor?.NotifyDropped(this);
       ownerInteractor = null;
       return false;
   }
}
