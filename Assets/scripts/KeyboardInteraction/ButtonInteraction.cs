using System;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteraction : InteractionBase
{
    public OnButtonPressed onButtonPressed;

    protected override void Start()
    {
        base.Start();
    }
    public override void Interact()
    {
        onButtonPressed.Invoke();
    }

}
[Serializable]
public class OnButtonPressed : UnityEvent { }
