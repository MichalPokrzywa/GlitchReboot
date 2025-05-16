using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance { get; private set; }

    public List<GhostController> ghosts { get; } = new List<GhostController>();
    public List<VariableDice> cubes { get; } = new List<VariableDice>();
    public List<VariablePlatform> platforms { get; } = new List<VariablePlatform>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Register(GameObject obj)
    {
        if (TryRegister<GhostController>(obj, ghosts)) return;
        if (TryRegister<VariableDice>(obj, cubes)) return;
        if (TryRegister<VariablePlatform>(obj, platforms)) return;
    }

    bool TryRegister<T>(GameObject obj, List<T> list) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component != null)
        {
            list.Add(component);
            Debug.Log(typeof(T).Name + " registered: " + component.name);
            return true;
        }
        return false;
    }
}
