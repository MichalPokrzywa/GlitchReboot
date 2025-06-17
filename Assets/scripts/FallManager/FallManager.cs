using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiceState
{
    public Vector3 startPosition;
    public Quaternion startRotation;
}


public class FallManager : MonoBehaviour
{
    public static FallManager Instance;
    public Transform currentCheckpoint;
    
    private Dictionary<GameObject, DiceState> diceData = new();
    
    void Awake() {
        Instance = this;
        CacheDiceObjects();
    }
    
    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
    }
    
    public void ResetPlayer(GameObject player) {
        player.transform.position = currentCheckpoint.position;
        player.transform.rotation = currentCheckpoint.rotation;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    public void ResetDice(GameObject dice)
    {
        if (diceData.TryGetValue(dice, out DiceState state))
        {
            dice.transform.position = state.startPosition;
            dice.transform.rotation = state.startRotation;

            Rigidbody rb = dice.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
    
    private void CacheDiceObjects()
    {
        int diceLayer = LayerMask.NameToLayer("VariableDice");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == diceLayer)
            {
                DiceState state = new DiceState
                {
                    startPosition = obj.transform.position,
                    startRotation = obj.transform.rotation
                };
                diceData[obj] = state;
            }
        }

        Debug.Log($"Zapisano {diceData.Count} kostek do indywidualnego resetu.");
    }
    
}
