using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalAssignedObjects : MonoBehaviour
{
    public List<ObjectOnLevel> objectsOnLevels = new List<ObjectOnLevel>();
}

[Serializable]
public class ObjectOnLevel
{
    public string name;
    public GameObject gameObject;
}
