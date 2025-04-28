using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public enum BlockType
    {
        Performer,
        Action,
        Object
    }

    public enum BlockParentType
    {
        SelectionBox,
        Sequence
    }

    [Serializable]
    public class BlockData
    {
        public string name;
        public BlockType type;
        public BlockParentType parentType;
    }

    public Action<Block> OnDragged;
    public BlockData BlockInfo => blockData;

    [SerializeField] BlockData blockData;
    [SerializeField] GameObject selectionIndicator;
    [SerializeField] Button button;

    bool selected = false;
    Vector3 offset;

    public void ToggleSelection()
    {
        selected = !selected;
        selectionIndicator.SetActive(selected);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnDragged?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out var globalMousePos
        );
        transform.position = globalMousePos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
