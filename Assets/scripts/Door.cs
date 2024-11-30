using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Condition condition;
    //[SerializeField] Player player;
    [SerializeField] GameObject startPosition;

    private BoxCollider boxCollider;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Kolizja!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Kolizja!");
    }

    public bool reset()
    {
        if (condition.returnCondition())
        {
            //end level
            return true;
        } else
        {
            resetPlayerPosition();
            return false;
        }          
    }

    public void resetPlayerPosition()
    {
        //player.transform.position = startPosition.transform.position;
    }
}
