using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Condition condition;
    //[SerializeField] Player player;
    //[SerializeField] GameObject start;



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

        return true;   
    }

    public void resetPlayerPosition()
    {
        //player.transform.position = start.transform.position;
    }
}
