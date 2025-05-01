using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockSequence : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image selectionIndicator;

    public Action<BlockSequence, PointerEventData> OnDropped;
    public int BlockCount => blocks.Count;

    List<Block> blocks = new List<Block>();

    void Awake()
    {
        SetSelection(false);
    }

    public void AddBlock(Block block)
    {
        blocks.Add(block);
    }

    public void RemoveBlock(Block block)
    {
        if (blocks.Contains(block))
        {
            blocks.Remove(block);
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
        SetSelection(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetSelection(false);
    }

    #endregion

    void SetSelection(bool selected)
    {
        SetImageAlpha(selectionIndicator, selected ? 1f : 0f);
    }

    static void SetImageAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
