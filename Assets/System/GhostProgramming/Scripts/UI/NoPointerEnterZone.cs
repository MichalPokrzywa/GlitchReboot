using UnityEngine;
using UnityEngine.EventSystems;

public class NoPointerEnterZone : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] SequenceManager sequenceManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        sequenceManager.OnNoDropZoneDropped();
    }
}
