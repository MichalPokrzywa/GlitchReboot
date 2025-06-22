using UnityEngine;

public class RepositionScript : MonoBehaviour
{
    public GameObject rePosition;
    
    public void OnCollisionEnter(Collision collision)
    {
        if (rePosition != null && collision.gameObject.CompareTag("Player")) 
        { 
            collision.gameObject.transform.position = rePosition.transform.position;
            collision.gameObject.transform.SetParent(null);
        }
        else if (collision.gameObject.GetComponent<ResetObject>() != null)
        {
            collision.gameObject.GetComponent<ResetObject>().ResetToInitialState();
        }

    }
}
