using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDiscardZone : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] SequenceManager sequenceManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        sequenceManager.DiscardSelectedBlock();
    }
}
