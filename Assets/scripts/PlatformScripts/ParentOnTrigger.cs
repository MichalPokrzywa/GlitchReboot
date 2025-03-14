using UnityEngine;

public class ParentOnTrigger : MonoBehaviour
{
    public CustomTrigger joiningPlatformTrigger;
    public CustomTrigger exitingPlatformTrigger;

    void Awake()
    {
        joiningPlatformTrigger.EnteredTrigger.AddListener(OnJoiningPlatformTrigger);
        exitingPlatformTrigger.ExitedTrigger.AddListener(OnExitingPlatformTrigger);
    }
    
    private void Reset()
    {
        EnsureTriggerExists(ref joiningPlatformTrigger, "JoiningPlatformTrigger");
        EnsureTriggerExists(ref exitingPlatformTrigger, "ExitingPlatformTrigger");
    }

    private void OnJoiningPlatformTrigger(Collider collider)
    {
        collider.transform.SetParent(transform);
    }

    private void OnExitingPlatformTrigger(Collider collider)
    {
        // Debug.Log($"Obiekt {collider.gameObject.name} wszedł w trigger {gameObject.name}, status dziecka:{IsMyDirectChild(collider.gameObject)}");
        if (IsMyDirectChild(collider.gameObject)) {collider.transform.SetParent(null);}
    }
    
    private bool IsMyDirectChild(GameObject obj)
    {
        return obj != null && obj.transform.parent == transform;
    }
    
    private void EnsureTriggerExists(ref CustomTrigger trigger, string triggerName)
    {
        if (trigger == null)
        {
            Transform existingTrigger = transform.Find(triggerName);
            
            if (existingTrigger == null)
            {
                GameObject triggerObject = new GameObject(triggerName);
                triggerObject.transform.SetParent(transform);
                triggerObject.transform.localPosition = Vector3.zero;
                triggerObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                Collider collider = triggerObject.AddComponent<BoxCollider>(); // Domyślnie BoxCollider
                collider.isTrigger = true;

                trigger = triggerObject.AddComponent<CustomTrigger>();

                Debug.Log($"Stworzono brakujący trigger: {triggerName}");
            }
            else
            {
                trigger = existingTrigger.GetComponent<CustomTrigger>();
            }
        }
    }
}
