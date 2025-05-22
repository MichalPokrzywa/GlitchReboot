using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class VariableChangePlatform : MonoBehaviour
{
    public VariableType type;

    public List<GameObject> platformType;

    private VariableDice dice = null;
    [SerializeField] private Transform dicePosition;
    void Start()
    {
        foreach (GameObject o in platformType)
        {
            o.SetActive(false);
        }
        switch (type)
        {
            case VariableType.Number:
                platformType[(int)VariableType.Number].SetActive(true);
                break;
            case VariableType.Boolean:
                platformType[(int)VariableType.Boolean].SetActive(true);
                break;
        }
    }

    public void ReceiveValue(VariableDice dice)
    {
        // Handle the received value
        if (type == dice.type)
        {
            Debug.Log($"Platform received {dice.type}");
            this.dice = dice;
        }
        else
        {
            Debug.LogError("Platform received wrong value type");
        }
    }

    public void UpdateValueUp()
    {
        if (dice == null) return;

        if (dice.type == VariableType.Number)
        {
            dice.UpdateValue((int)dice.GetCurrentValue() + 1);
        }
        if (dice.type == VariableType.Boolean)
        {
            dice.UpdateValue(!(bool)dice.GetCurrentValue());
        }
    }

    public void UpdateValueDown()
    {
        if (dice == null) return;

        if (dice.type == VariableType.Number)
        {
            dice.UpdateValue((int)dice.GetCurrentValue() - 1);
        }
    }

    public void EndModification()
    {
        dice = null;
    }

    public void MoveObjectToPosition(GameObject gameObject)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.
            gameObject.transform.DOMove(dicePosition.position, 0.3f).OnComplete(() =>
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = true;
            });
    }
}
