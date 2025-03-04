using UnityEngine;

public class MarkerScript : MonoBehaviour
{
    void Awake()
    {
        this.gameObject.SetActive(false);
    }

    public void Activate(Vector3 pos)
    {
        this.transform.position = pos;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
