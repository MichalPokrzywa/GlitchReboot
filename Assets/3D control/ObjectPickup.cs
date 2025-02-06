using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [SerializeField]
    private Transform holdPoint; // Where the picked object will be held

    private GameObject pickedObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to pick up or drop the object
        {
            if (pickedObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    private void TryPickupObject()
    {
        // Cast a ray from the player's position to detect a pickupable object
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f)) // Adjust ray length if needed
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("VariableDice"))
            {
                pickedObject = hit.collider.gameObject;
                PickUpObject(pickedObject);
            }
        }
    }

    private void PickUpObject(GameObject obj)
    {
        Debug.Log("dzieckuje obiekt");
        obj.transform.SetParent(holdPoint); // Parent to the hold point
        obj.GetComponent<Rigidbody>().detectCollisions = false;
        obj.GetComponent<Rigidbody>().isKinematic = true; // Disable physics
        obj.transform.position = holdPoint.position; // Move to hold point
        obj.transform.rotation = holdPoint.rotation; // Align rotation with hold point
    }

    private void DropObject()
    {
        pickedObject.transform.SetParent(null); // Unparent the object
        pickedObject.GetComponent<Rigidbody>().isKinematic = false; // Enable physics
        pickedObject.GetComponent<Rigidbody>().detectCollisions = true;
        pickedObject = null; // Clear reference
    }
}