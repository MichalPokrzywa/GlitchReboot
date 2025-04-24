using UnityEngine;

public class PickUpObjectInteraction : InteractionBase
{
   // TODO testy, zrobić aby hold point automatycznie łapał hold point czy coś,
   Transform holdPoint;

   [SerializeField] Rigidbody rb;

   bool iAmPickedUp;

   private void Start()
   {
       if (rb == null)
        rb = GetComponent<Rigidbody>();

      iAmPickedUp = false;
   }

   public override void Interact(Transform? holdPoint = null)
   {
       if (holdPoint != null)
        this.holdPoint = holdPoint;

       if (iAmPickedUp)
       {
          DropMe();
       }
       else
       {
          PickMeUp();
       }
   }


   private void FixedUpdate()
   {
       if (iAmPickedUp)
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

   private void PickMeUp()
   {
       rb.useGravity = false;
       rb.linearDamping = 10;
       rb.constraints = RigidbodyConstraints.FreezeRotation;

       /*transform.parent = holdPoint;*/
       iAmPickedUp = true;
   }

   private void DropMe()
   {
        rb.useGravity = true;
        rb.linearDamping = 1;
        rb.constraints = RigidbodyConstraints.None;
        /*transform.parent = null;*/
        iAmPickedUp = false;
   }
}
