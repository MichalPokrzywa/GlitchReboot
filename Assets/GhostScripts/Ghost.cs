using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghost", menuName = "Scriptable Objects/Ghost")]
public class Ghost : ScriptableObject
{
    public bool isRecord;
    public bool isReplay;
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
}
