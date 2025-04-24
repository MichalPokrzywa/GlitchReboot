using UnityEngine;

public class PickUpObjectInteraction : InteractionBase
{   
   public Transform holdPoint;
   private Rigidbody rb;
   public bool IsPickedUp { get; private set; }

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
           //rb.AddForce((holdPoint.position - transform.position) * (10/ Time.fixedDeltaTime));
           rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, holdPoint.rotation, 4));
       }
   }

   public void PickMeUp(Transform point)
   {
       holdPoint = point;
       rb.useGravity = false;
       rb.linearDamping = 10;
       rb.constraints = RigidbodyConstraints.FreezeRotation;
       IsPickedUp = true;
   }

   public void DropMe()
   {
       holdPoint = null;
       rb.useGravity = true;
       rb.linearDamping = 1;
       rb.constraints = RigidbodyConstraints.None;
       IsPickedUp = false;
   }
}
