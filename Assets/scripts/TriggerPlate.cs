using UnityEngine;
public interface IActivatable
{
    void Activate();
    void Deactivate(){}
}
public class TriggerPlate : MonoBehaviour
{
    public GameObject objectToActivate; // Obiekt do aktywacji
    private GameObject triggerBox; // Dziecko jako trigger
    private IActivatable activatable;

    void Start()
    {
        activatable = objectToActivate?.GetComponent<IActivatable>();
        if (activatable == null)
        {
            Debug.LogError("Activable component not found!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        activatable.Activate();
    }

    void OnTriggerExit(Collider other)
    {
        activatable.Deactivate();
    }
}