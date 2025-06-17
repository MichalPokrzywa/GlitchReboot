using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class VariableChangePlatform : VariablePlatformBase
{
    public List<GameObject> platformType;

    [SerializeField] private Transform dicePosition;

    private VariableDice dice = null;

    public override void Start()
    {
        base.Start();
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
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = VariableTypeColor.GetColor(type);
        UpdateEntityNameSuffix();
    }

    public override void ReceiveValue(object obj)
    {
        if (obj is not VariableDice dice)
            return;

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

    public override void ClearValue()
    {
        dice = null;
        assignedObject = null;
    }

    public override void MoveObjectToPosition(GameObject go)
    {
        Rigidbody rb = go.GetComponent<Rigidbody>();
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

    public override void AssignObjectToPlatform(GameObject go)
    {
        if (assignedObject == null)
        {
            assignedObject = go;
        }
    }

    public override void UpdateEntityNameSuffix()
    {
        string color = VariableTypeColor.GetHex(type);
        entityNameSuffix = $"Value change <b><color={color}>{type.ToString()}</color></b>";
    }
}
