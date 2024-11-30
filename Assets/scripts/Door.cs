using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Condition condition;
    //[SerializeField] Player player;
    [SerializeField] GameObject startPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
