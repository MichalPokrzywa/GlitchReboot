using UnityEngine;

public class PickUpObjectInteraction : InteractionBase
{   
   // TODO testy, zrobić aby hold point automatycznie łapał hold point czy coś, 
   private Transform holdPoint;
   bool iAmPickedUp = false;
   
   public new void Interact()
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
      GetComponent<Rigidbody>().detectCollisions = false;
      GetComponent<Rigidbody>().isKinematic = true; // Disable physics
      transform.position = holdPoint.position; // Move to hold point
      transform.rotation = holdPoint.rotation; // Align rotation with hold point
      iAmPickedUp = true;
   }
   
   private void DropMe()
   {
      transform.SetParent(null); // Unparent the object
      GetComponent<Rigidbody>().isKinematic = false; // Enable physics
      GetComponent<Rigidbody>().detectCollisions = true;
      iAmPickedUp = false; // Clear reference
   }
}
