using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum MechanicType
{
    None,
    PickUp,
    GhostProgram,
    MarkerSpawn,
}

public class MechanicsManager : Singleton<MechanicsManager>
{
    [SerializeField] AllScenesMechanicsConfig config;

    HashSet<MechanicType> activeMechanics = new();

    void Start()
    {
        DependencyManager.sceneLoader.sceneLoaded.AddListener(OnSceneLoaded);
    }

    void OnSceneLoaded()
    {
        EnableAll();

#if UNITY_EDITOR
        return;
#endif

        Scene currentScene = DependencyManager.sceneLoader.CurrentScene;
        var entry = config.sceneMechanicEntries.Find(c => c.scene == currentScene);
        if (entry != null)
        {
            foreach (var mechanic in entry.initiallyDisabled)
                Disable(mechanic);
        }

    }

    public bool IsEnabled(MechanicType mechanic)
    {
        if (mechanic == MechanicType.PickUp)
            File.WriteAllText(Application.persistentDataPath + "/editor_check.txt", DependencyManager.sceneLoader.CurrentScene.ToString());

#if UNITY_EDITOR
        return true;
#else
        return activeMechanics.Contains(mechanic);
#endif
    }

    public void Enable(MechanicType mechanic) => activeMechanics.Add(mechanic);

    public void Disable(MechanicType mechanic) => activeMechanics.Remove(mechanic);

    void EnableAll()
    {
        foreach (MechanicType type in System.Enum.GetValues(typeof(MechanicType)))
        {
            if (type == MechanicType.None)
                continue;

            activeMechanics.Add(type);
        }
    }
}