using TMPro;
using UnityEngine;

public class MarkerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI markerText;
    [SerializeField] Transform target;

    public bool isActive => gameObject.activeSelf;

    void Awake()
    {
        EntityManager.instance.Register(gameObject);
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        // billboard behaviour
        if (target != null)
        {
            transform.LookAt(target.transform);
            transform.Rotate(0, 180, 0);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
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

    public void SetText(string text)
    {
        markerText.text = text;
    }
}
