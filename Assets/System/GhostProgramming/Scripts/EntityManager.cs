using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    readonly Dictionary<Type, IList> entityLists = new()
    {
        { typeof(GhostController), new List<GhostController>() },
        { typeof(VariableDice), new List<VariableDice>() },
        { typeof(VariablePlatform), new List<VariablePlatform>() },
        { typeof(MarkerScript), new List<MarkerScript>() }
    };

    void Awake()
    {
        SceneManager.sceneUnloaded += ClearEntities;
    }

    void OnDestroy()
    {
        SceneManager.sceneUnloaded -= ClearEntities;
    }

    public void Register<T>(T entity) where T : EntityBase
    {
        var list = GetList<T>();
        if (list == null)
        {
            Debug.LogWarning($"[EntityManager] Unsupported entity type: {typeof(T).Name}");
            return;
        }

        int finalId = entity.entityId > 0 ? entity.entityId : 1;

        var usedIds = list.Select(e => e.entityId).ToHashSet();
        while (usedIds.Contains(finalId))
            finalId++;

        entity.entityId = finalId;
        list.Add(entity);

        Debug.Log($"[EntityManager] Registered {typeof(T).Name} \"{entity.name}\" with ID {finalId} and suffix \"{entity.entityNameSuffix}\"");
    }

    public  List<T> GetEntities<T>() where T : EntityBase
    {
        return GetList<T>() ?? new List<T>();
    }

    List<T> GetList<T>() where T : EntityBase
    {
        if (entityLists.TryGetValue(typeof(T), out var list))
            return list as List<T>;
        return null;
    }

    void ClearEntities(UnityEngine.SceneManagement.Scene arg0)
    {
        entityLists.Clear();
    }
}
