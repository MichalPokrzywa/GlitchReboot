using GhostProgramming;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockSequence : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] Toggle toggle;
    [SerializeField] Color normalColor;
    [SerializeField] Color selectedColor;

    public Action<BlockSequence, PointerEventData> OnDropped;
    public int BlockCount => blocks.Count;
    public bool IsSelected => selected;
    public bool IsRunning = false;
    public List<Block> Blocks => blocks;

    List<Block> blocks = new List<Block>();
    bool selected = false;

    void Awake()
    {
        UpdateHighlight(false);
        toggle.onValueChanged.AddListener(OnToggle);
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggle);
    }

    void OnToggle(bool isOn)
    {
        selected = isOn;
        image.color = selected ? selectedColor : normalColor;
    }

    public void AddBlock(int index, Block block)
    {
        blocks.Insert(index, block);
        UpdateNodeLinks();
    }

    public void RemoveBlock(Block block)
    {
        if (blocks.Contains(block))
        {
            blocks.Remove(block);
            UpdateNodeLinks();
        }
        else
        {
            Debug.LogWarning("Block not found in this sequence.");
        }
    }

    public bool ContainsBlock(Block block)
    {
        return blocks.Contains(block);
    }

    #region InterfacesImplementation

    public void OnDrop(PointerEventData eventData)
    {
        OnDropped?.Invoke(this, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selected)
            return;

        UpdateHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selected)
            return;

        UpdateHighlight(false);
    }

    #endregion

    void UpdateNodeLinks()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            Node current = blocks[i].BlockNode;
            Node prev = i > 0 ? blocks[i - 1].BlockNode : null;
            Node next = i < blocks.Count - 1 ? blocks[i + 1].BlockNode : null;

            current.prevNode = prev;
            current.nextNode = next;
        }
    }

    void UpdateHighlight(bool highlighted)
    {
        SetImageAlpha(image, highlighted ? 1f : 0f);
    }

    static void SetImageAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
