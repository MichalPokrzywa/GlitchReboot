using System;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteraction : InteractionBase
{
    public OnButtonPressed onButtonPressed;
    public string buttonActionName;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        base.Awake();
        TooltipText = "[E] " + buttonActionName;
    }
    public override void Interact()
    {
        onButtonPressed.Invoke();
    }

}
[Serializable]
public class OnButtonPressed : UnityEvent { }
