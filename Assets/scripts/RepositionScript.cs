using UnityEngine;

public class RepositionScript : MonoBehaviour
{
    public GameObject rePosition;
    
    public void OnCollisionEnter(Collision collision)
    {
        if (rePosition != null && collision.gameObject.CompareTag("Player")) 
        { 
            collision.gameObject.transform.position = rePosition.transform.position;
        }
    }
}
