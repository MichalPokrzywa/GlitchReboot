using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    static EntityManager _instance;

    public static EntityManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EntityManager>();
                if (_instance == null)
                {
                    GameObject singletonGO = new GameObject("EntityManager (Singleton)");
                    _instance = singletonGO.AddComponent<EntityManager>();
                }
            }

            return _instance;
        }
    }

    public List<GhostController> ghosts { get; } = new List<GhostController>();
    public List<VariableDice> cubes { get; } = new List<VariableDice>();
    public List<VariablePlatform> platforms { get; } = new List<VariablePlatform>();
    public List<MarkerScript> markers { get; } = new List<MarkerScript>();

    public void Register(GameObject obj)
    {
        if (TryRegister<GhostController>(obj, ghosts)) return;
        if (TryRegister<VariableDice>(obj, cubes)) return;
        if (TryRegister<VariablePlatform>(obj, platforms)) return;
        if (TryRegister<MarkerScript>(obj, markers)) return;
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
