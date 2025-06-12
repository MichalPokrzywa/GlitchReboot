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
    [SerializeField] private TextColorLooper colorLooper;
    private IVariableValueHandler handler;
    bool onPlatform = false;

    void Start()
    {
        InitializeHandler();
        colorLooper = GetComponent<TextColorLooper>();
        colorLooper.textList = textList;
        UpdateEntityNameSuffix();
        EntityManager.Instance.Register<VariableDice>(this);
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
        if (other.gameObject.layer == LayerMask.NameToLayer("VariablePlatform")
            || other.gameObject.layer == LayerMask.NameToLayer("VariableChangePlatform"))
        {
            var pickUpObject = GetComponent<PickUpObjectInteraction>();
            VariablePlatformBase platform = other.gameObject.GetComponent<VariablePlatformBase>();

            // if dice is ON the platform (within the platform and not picked up by someone), assign the value
            if (platform != null && !onPlatform && pickUpObject.IsDropped)
            {
                if (platform.type != type)
                    return;

                if(platform.assignedObject != null)
                    return;

                object currentValue = this;
                if (platform is VariablePlatform)
                    currentValue = handler?.GetValue();

                Debug.Log("ON PLATFORM!");
                onPlatform = true;
                platform.ReceiveValue(currentValue);
                platform.AssignObjectToPlatform(this.gameObject);
                platform.MoveObjectToPosition(this.gameObject);
                colorLooper.StartLoop();
            }
            // if dice is still within the platform but not ON the platform (e.g. picked up by someone), clear the value
            else if (platform != null && onPlatform && !pickUpObject.IsDropped)
            {
                DetachFromPlatform(platform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the other object is on the correct layer
        if (other.gameObject.layer == LayerMask.NameToLayer("VariablePlatform")
            || other.gameObject.layer == LayerMask.NameToLayer("VariableChangePlatform"))
        {
            VariablePlatformBase platform = other.gameObject.GetComponent<VariablePlatformBase>();
            if (platform != null && onPlatform)
            {
                DetachFromPlatform(platform);
            }
        }
    }

    void DetachFromPlatform(VariablePlatformBase platform)
    {
        if (platform.assignedObject == this.gameObject)
        {
            Debug.Log("EXIT PLATFORM!");
            onPlatform = false;
            platform.ClearValue();
            colorLooper.StopLoop();
        }
    }

}