using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSequence : MonoBehaviour
{
    [SerializeField] Image selectionIndicator;

    public int Index => _index;

    int _index = 0;
    bool selected = false;
    List<Block> blocks = new List<Block>();

    public void Initialize(int index, Block block)
    {
        ToggleSelection();
        _index = index;
        AddBlock(block);
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    public void ToggleSelection()
    {
        selected = !selected;
        selectionIndicator.enabled = selected;
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
            Destroy(block.gameObject);
        }
        else
        {
            Debug.LogWarning("Block not found in this sequence.");
        }
    }
}
