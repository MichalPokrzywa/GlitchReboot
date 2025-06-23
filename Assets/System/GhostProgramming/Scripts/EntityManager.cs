using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityManager : Singleton<EntityManager>
{
    Dictionary<Type, IList> entityLists = new()
    {
        { typeof(GhostController), new List<GhostController>() },
        { typeof(VariableDice), new List<VariableDice>() },
        { typeof(VariablePlatformBase), new List<VariablePlatformBase>() },
        { typeof(MarkerScript), new List<MarkerScript>() }
    };

    void Awake()
    {
        SceneManager.sceneUnloaded += ClearEntities;
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

        //sort based on id
        entityLists[typeof(T)] = list.OrderBy(e => e.entityId).ToList();

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
        entityLists = new()
        {
            { typeof(GhostController), new List<GhostController>() },
            { typeof(VariableDice), new List<VariableDice>() },
            { typeof(VariablePlatformBase), new List<VariablePlatformBase>() },
            { typeof(MarkerScript), new List<MarkerScript>() }
        };
    }
}
