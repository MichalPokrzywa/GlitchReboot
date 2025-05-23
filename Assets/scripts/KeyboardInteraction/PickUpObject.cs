using DG.Tweening;
using System.Drawing;
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
   private bool onTarget = false;
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

   private void Update()
   {
       if (!IsPickedUp && HasShownUI)
       {
           AnimateUI();
       }
       if (IsPickedUp)
       {
            if (handPoint  != null) 
            {
                if (Vector3.Distance(holdPoint.position, transform.position) > 0.2f && !onTarget)
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
                    rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, handPoint.rotation, Time.deltaTime * rotationSpeed));
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
       transform.DOScale(0.3f, 0.2f).OnComplete(() =>
       {
           rb.linearDamping = 100;
           rb.angularDamping = 10;
           rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
           IsPickedUp = true;
       });
       handPoint = handTransform;
       holdPoint = dropPoint;
       ownerInteractor = interactor;
       onTarget = false;
   }

   public void DropInFront()
   {
       transform.DOMove(holdPoint.position, 0.1f);
       transform.DOScale(1f, 0.1f).OnComplete(() =>
       {
           new WaitForSeconds(0.1f);
           handPoint = null;
           holdPoint = null;
           // reset physics
           rb.useGravity = true;
           rb.linearDamping = 1;
           rb.constraints = RigidbodyConstraints.None;
           IsPickedUp = false;
           onTarget = false;
           // let the interactor know to clear its heldObject
           ownerInteractor?.NotifyDropped(this);
           ownerInteractor = null;
       });
   }

   public void Throw()
   {
       transform.DOMove(holdPoint.position, 0.05f);
       transform.DOScale(1f, 0.05f).OnComplete(() =>
       {
           handPoint = null;
           holdPoint = null;
           rb.useGravity = true;
           rb.linearDamping = 1f;
           rb.angularDamping = 0.05f;
           rb.constraints = RigidbodyConstraints.None;
           IsPickedUp = false;
           onTarget = false;

           Vector3 dir = mainCamera.transform.forward;
           rb.AddForce(dir * 30f, ForceMode.Impulse);
       });
    }


   public void PickMeUp(Transform point, Interactor interactor)
   {
       holdPoint = point;
       ownerInteractor = interactor;

       rb.useGravity = false;
       rb.linearDamping = 10;
       rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
       IsPickedUp = true;
   }

   public void DropMe()
   {
       // reset physics
       rb.useGravity = true;
       rb.linearDamping = 1;
       rb.constraints = RigidbodyConstraints.None;
       IsPickedUp = false;

       // let the interactor know to clear its heldObject
       ownerInteractor?.NotifyDropped(this);
       ownerInteractor = null;
   }
}
