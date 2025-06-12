using UnityEngine;

public abstract class VariablePlatformBase : EntityBase
{
    [Tooltip("Choose the variable type here")]
    public VariableType type;
    public GameObject assignedObject;

    public virtual void Start()
    {
        EntityManager.Instance.Register<VariablePlatformBase>(this);
    }
    public abstract void ReceiveValue(object obj);
    public abstract void ClearValue();
    public abstract void MoveObjectToPosition(GameObject go);
    public abstract void AssignObjectToPlatform(GameObject go);
}
