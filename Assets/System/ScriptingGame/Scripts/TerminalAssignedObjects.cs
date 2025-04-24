using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalAssignedObjects : MonoBehaviour
{
    public List<ObjectOnLevel> objectsOnLevels = new List<ObjectOnLevel>();

    public void ResetObjects()
    {
        foreach (var objects in objectsOnLevels)
        {
            var reset = objects.gameObject.GetComponent<ResetObject>();

            if (reset != null)
            {
                reset.ResetToInitialState();
            }
            else
            {
                Debug.Log($"Object {objects.name} doesn't have a ResetObject script.");
            }
        }
    }
}

[Serializable]
public class ObjectOnLevel
{
    public string name;
    public GameObject gameObject;
}
