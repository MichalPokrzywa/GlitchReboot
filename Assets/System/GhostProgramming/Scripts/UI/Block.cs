using System;
using System.Collections.Generic;
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

    bool selected = false;
    Vector3 offset;
    List<Image> images = new List<Image>();

    void Awake()
    {
        images = new List<Image>(GetComponentsInChildren<Image>(true));
    }

    public void UpdateParentType(BlockParentType parentType)
    {
        blockData.parentType = parentType;
    }

    public void UpdateSelection(bool select)
    {
        selected = select;
        selectionIndicator.SetActive(select);
    }

    public void RaycastTargetActivation(bool active)
    {
        foreach (var img in images)
        {
            img.raycastTarget = active;
        }
    }

    #region InterfacesImplementation

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

    #endregion
}
