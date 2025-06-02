using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class VariableDice : EntityBase
{
    public VariableType type;
    public List<TMP_Text> textList;

    [SerializeField] private int baseNumberValue = 1; // Initial value for Number
    [SerializeField] private bool baseBooleanValue = false; // Initial value for Boolean
    [SerializeField] private string baseStringValue = ""; // Initial value for Boolean
    [SerializeField] private GameObject baseGameObjectValue; // Initial value for Boolean

    private IVariableValueHandler handler;

    void Awake()
    {
        EntityManager.instance.Register<VariableDice>(this);
    }

    void Start()
    {
        InitializeHandler();
        UpdateEntityNameSuffix();
    }

    public override void UpdateEntityNameSuffix()
    {
        entityNameSuffix = ": " + GetCurrentValue().ToString();
    }

    void InitializeHandler()
    {
        switch (type)
        {
            case VariableType.Number:
                handler = new NumberHandler(baseNumberValue);
                UpdateValue(baseNumberValue);
                break;
            case VariableType.Boolean:
                handler = new BooleanHandler(baseBooleanValue);
                UpdateValue(baseBooleanValue);
                break;
            case VariableType.String:
                handler = new StringHandler(baseStringValue);
                UpdateValue(baseStringValue);
                break;
            case VariableType.GameObject:
                handler = new GameObjectHandler(baseGameObjectValue);
                UpdateValue(baseGameObjectValue);
                break;
        }
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = VariableTypeColor.GetColor(type);
    }

    public object GetCurrentValue()
    {
        if (handler == null)
        {
            Debug.LogWarning("Handler not initialized!");
            return null;
        }

        return handler.GetValue();
    }

    public void UpdateValue(object value)
    {
        handler.UpdateValue(value);
        foreach (TMP_Text text in textList)
        {
            handler.UpdateTextValue(text,false);
        }
        UpdateEntityNameSuffix();
    }

    private void OnTriggerStay(Collider other)
    {
        //DependencyManager.audioManager.PlaySound(Sound.None);
        // Check if the other object is on the correct layer
        if (other.gameObject.layer == LayerMask.NameToLayer("VariablePlatform"))
        {
            // Check if the other object has a VariablePlatform component
            VariablePlatform platform = other.gameObject.GetComponent<VariablePlatform>();
            if (platform != null)
            {
                var pickUpObject = GetComponent<PickUpObjectInteraction>();
                if (pickUpObject.IsDropped)
                {
                    platform.MoveObjectToPosition(this.gameObject);
                    // Get the dice's current value and send it to the platform
                    object currentValue = handler?.GetValue();
                    platform.ReceiveValue(currentValue);
                }
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("VariableChangePlatform"))
        {
            VariableChangePlatform platform = other.gameObject.GetComponent<VariableChangePlatform>();
            if (platform != null)
            {
                var pickUpObject = GetComponent<PickUpObjectInteraction>();
                if (pickUpObject.IsDropped)
                {
                    platform.MoveObjectToPosition(this.gameObject);
                    // Get the dice and send it to the platform
                    platform.ReceiveValue(this);
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the other object is on the correct layer
        if (other.gameObject.layer == LayerMask.NameToLayer("VariablePlatform"))
        {
            // Check if the other object has a VariablePlatform component
            VariablePlatform platform = other.gameObject.GetComponent<VariablePlatform>();
            if (platform != null)
            {
                // Clear the value on the platform when the trigger is exited
                platform.ClearValue();
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("VariableChangePlatform"))
        {
            VariableChangePlatform platform = other.gameObject.GetComponent<VariableChangePlatform>();
            if (platform != null)
            {
                // Get the dice and send it to the platform
                platform.EndModification();
            }
        }
    }
}