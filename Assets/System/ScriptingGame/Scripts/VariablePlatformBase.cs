using UnityEngine;

public abstract class VariablePlatformBase : EntityBase
{
    public virtual void Awake()
    {
        EntityManager.instance.Register<VariablePlatformBase>(this);
    }
    public abstract void ReceiveValue(object obj);
    public abstract void ClearValue();
    public abstract void MoveObjectToPosition(GameObject go);
}
