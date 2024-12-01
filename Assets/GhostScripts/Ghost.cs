using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghost", menuName = "Scriptable Objects/Ghost")]
public class Ghost : ScriptableObject
{
    private bool isRecord;
    private bool isReplay;
    public float recordFrequancy;

    public List<float> timeStamp;
    public List<Vector3> position;
    public List<Vector3> rotation;

    public void RestetData()
    {
        timeStamp.Clear();
        position.Clear();
        rotation.Clear();
    }

    public void setIsRecord(bool condition)
    {
        isRecord = condition;
    }
    public void setIsReplay(bool condition)
    {
        isReplay = condition;
    }
    public bool getIsRecord()
    {
        return isRecord;
    }
    public bool getIsReplay()
    {
        return isReplay;
    }
}
