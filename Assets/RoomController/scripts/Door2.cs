using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.Log(SceneManager.GetActiveScene().name);
            //end level
            if (SceneManager.GetActiveScene().name.ToString() == "level1") SceneManager.LoadScene("level2");
            if (SceneManager.GetActiveScene().name.ToString() == "level2") SceneManager.LoadScene("level3");
            if (SceneManager.GetActiveScene().name.ToString() == "level3") SceneManager.LoadScene("EndScreen");
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
