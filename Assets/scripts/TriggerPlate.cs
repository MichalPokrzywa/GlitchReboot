using UnityEngine;
public interface IActivatable
{
    void Activate();
}
public class TriggerPlate : MonoBehaviour
{
    public GameObject objectToActivate; // Obiekt do aktywacji
    private GameObject triggerBox; // Dziecko jako trigger

    void Reset()
    {
        // Tworzenie dziecka jako triggera
        triggerBox = new GameObject("TriggerBox");
        triggerBox.transform.SetParent(transform);
        triggerBox.transform.localPosition = Vector3.zero;
        triggerBox.transform.localScale = Vector3.one;
        
        Collider collider = triggerBox.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        IActivatable activatable = objectToActivate?.GetComponent<IActivatable>();
        if (activatable != null)
        {
            activatable.Activate();
        }
    }
}