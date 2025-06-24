using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mechanics/ScenesMechanicsConfig")]
public class AllScenesMechanicsConfig : ScriptableObject
{
    public List<SceneMechanicEntry> sceneMechanicEntries;
}

[System.Serializable]
public class SceneMechanicEntry
{
    public Scene scene;
    public List<MechanicType> initiallyDisabled;
}