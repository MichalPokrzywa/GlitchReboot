using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    [Tooltip("If the variable is not set here, it will dynamically take the next valid value when registering entities")]
    public int entityId;
    [HideInInspector] public string entityNameSuffix = string.Empty;

    public virtual void UpdateEntityNameSuffix() {}
}
