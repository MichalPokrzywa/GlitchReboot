using UnityEngine;

public class PickUpObjectInteraction : InteractionBase
{   
   // TODO testy, zrobić aby hold point automatycznie łapał hold point czy coś, 
   public Transform holdPoint;
   bool iAmPickedUp;
   private Rigidbody rb;

   private void Start()
   {
      rb = GetComponent<Rigidbody>();
      iAmPickedUp = false;
   }

   public override void Interact()
   {
      if (iAmPickedUp)
      {
         DropMe();
      }
      else
      {
         PickMeUp();
      }
   }
   
   private void PickMeUp()
   {
      transform.SetParent(holdPoint); // Parent to the hold point
      rb.isKinematic = true; // Disable physics
      transform.position = holdPoint.position; // Move to hold point
      transform.rotation = holdPoint.rotation; // Align rotation with hold point
      iAmPickedUp = true;
   }
   
   private void DropMe()
   {
      transform.SetParent(null); // Unparent the object
      rb.isKinematic = false; // Enable physics
      iAmPickedUp = false; // Clear reference
   }
}
