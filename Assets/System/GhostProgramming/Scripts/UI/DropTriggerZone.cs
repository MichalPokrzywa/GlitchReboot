using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropTriggerZone : MonoBehaviour, IDropHandler
{
    public Action OnDropped;

    public void OnDrop(PointerEventData eventData)
    {
        OnDropped?.Invoke();
    }
}
