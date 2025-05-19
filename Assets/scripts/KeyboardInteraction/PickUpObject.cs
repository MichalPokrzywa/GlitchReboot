using UnityEngine;

public class PickUpObjectInteraction : InteractionBase
{
   private Transform holdPoint;
   private Rigidbody rb;
   public bool IsPickedUp { get; private set; }
   [Header("Rotation Snap")]
   [Tooltip("Speed (deg/sec) at which to rotate toward the snapped orientation.")]
   public float rotationSpeed = 20f;
   private Interactor ownerInteractor;

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

   private void FixedUpdate()
   {
       if (!IsPickedUp && HasShownUI)
       {
           AnimateUI();
       }
       if (IsPickedUp)
       {
           if (Vector3.Distance(holdPoint.position, transform.position) > 4f)
           {
               transform.position = holdPoint.position;
           }
            //TODO: Do koloru do wyboru
            rb.linearVelocity = (holdPoint.position - transform.position) * (1 / Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, holdPoint.rotation, rotationSpeed) );
            //rb.AddForce((holdPoint.position - transform.position) * (10/ Time.fixedDeltaTime));
       }
   }

   public void PickMeUp(Transform point, Interactor interactor)
   {
       holdPoint = point;
       ownerInteractor = interactor;

       rb.useGravity = false;
       rb.linearDamping = 10;
       rb.constraints = RigidbodyConstraints.FreezeRotation;
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
