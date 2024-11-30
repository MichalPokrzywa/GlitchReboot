using UnityEngine;

public class Door2 : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject startPosition;

    private BoxCollider boxCollider;
    private Condition condition;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        condition = GetComponent<Condition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Kolizja!");
        reset();
    }

    public bool reset()
    {
        if (condition.returnCondition())
        {
            //end level
            return true;
        }
        else
        {
            resetPlayerPosition();
            return false;
        }
    }

    public void resetPlayerPosition()
    {
        Debug.Log("Reset!");
        player.transform.position = startPosition.transform.position;
    }
}
