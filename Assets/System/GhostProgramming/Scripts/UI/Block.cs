using System;
using System.Collections.Generic;
using GhostProgramming;
using JetBrains.Annotations;
using TMPro;
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
    public Node BlockNode => node;

    [SerializeField] BlockData blockData;
    [SerializeField] GameObject selectionIndicator;
    [SerializeField] TextMeshProUGUI header;
    [SerializeField] TextMeshProUGUI blockName;
    [SerializeField] [CanBeNull] TextMeshProUGUI inLabel;
    [SerializeField] [CanBeNull] TextMeshProUGUI outLabel;
    [SerializeField] BlockUIData blockUIData;
    [SerializeField] UIColorEcho uiColorEcho;

    bool selected = false;
    bool interactable = true;
    Node node;
    Vector3 offset;
    List<Image> images = new List<Image>();

    void Awake()
    {
        images = new List<Image>(GetComponentsInChildren<Image>(true));
        node = GetComponent<Node>();
    }

    void Update()
    {
        // update raycast based on the node's state (running or not)
        // only if the block is interactable
        if (interactable)
            RaycastTargetActivation(!node.isInRunningSequence);

        uiColorEcho.active = node.currentlyExecuting;
    }

    void OnValidate()
    {
        header.text = blockUIData.GetBlockName(blockData.type);
        header.color = blockUIData.GetHeaderColor(blockData.type);
        if (inLabel != null) inLabel.color = blockUIData.GetInLabelColor(blockData.type);
        if (outLabel != null) outLabel.color = blockUIData.GetOutLabelColor(blockData.type);
        if (blockName != null) blockName.text = blockData.name;
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

    public void UpdateInteractability(bool interactable)
    {
        this.interactable = interactable;
        RaycastTargetActivation(interactable);
    }

    #region InterfacesImplementation

    public void OnBeginDrag(PointerEventData eventData)
    {
        UpdateInteractability(false);
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
        UpdateInteractability(true);
    }

    void RaycastTargetActivation(bool active)
    {
        foreach (var img in images)
        {
            img.raycastTarget = active;
        }
    }

    #endregion
}
