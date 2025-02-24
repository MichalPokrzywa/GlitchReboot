using UnityEngine;

public class MarkerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    public void setActive(Vector3 pos)
    {
        this.transform.position = pos;
        gameObject.SetActive(true);
    }
    public void deactivate()
    {
        gameObject.SetActive(false);
    }
}
