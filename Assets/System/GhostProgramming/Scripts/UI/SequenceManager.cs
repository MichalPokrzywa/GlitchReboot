using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SequenceManager : MonoBehaviour, IDropHandler
{
    [SerializeField] BlockSequence sequencePrefab;
    [SerializeField] GameObject sequenceContainer;
    [SerializeField] GameObject selectionBox;
    [SerializeField] GameObject temporaryBlockPlacement;
    [SerializeField] List<RectTransform> uiToUpdate;

    List<BlockSequence> sequences = new List<BlockSequence>();
    List<Block> blocksInSelectionBox = new List<Block>();
    List<Block> blocksInSequences = new List<Block>();

   Block selectedBlock;

    void Awake()
    {
        InitBlocksFromSelectionBox();
    }

    void InitBlocksFromSelectionBox()
    {
        foreach (Transform child in selectionBox.transform)
        {
            var block = child.GetComponent<Block>();
            if (block != null)
            {
                blocksInSelectionBox.Add(block);
                block.OnDragged += OnBlockFromSelectionBoxDragged;
            }
        }
    }

    void OnBlockFromSelectionBoxDragged(Block block)
    {
        // Make currently dragged block a child of the temporary block placement
        // to display it on the top of all other UI elements
        block.transform.SetParent(temporaryBlockPlacement.transform);

        // Create copy of the block
        int index = blocksInSelectionBox.IndexOf(block);
        var blockCopy = Instantiate(block.gameObject, selectionBox.transform);
        // and place it in the same position as the original block
        blockCopy.transform.SetSiblingIndex(index);

        // Swap the original block with the copy in the list
        Block blockCopyComponent = blockCopy.GetComponent<Block>();
        blocksInSelectionBox[index] = blockCopyComponent;
        blockCopyComponent.OnDragged += OnBlockFromSelectionBoxDragged;

        SelectBlock(block);

        Debug.Log($"Selected {selectedBlock}");
    }

    void OnBlockFromSequenceDragged(Block block)
    {

    }

    void SelectBlock(Block block)
    {
        selectedBlock?.ToggleSelection();
        selectedBlock = block;
        selectedBlock.ToggleSelection();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Instantiate sequence
        var go = Instantiate(sequencePrefab.gameObject, sequenceContainer.transform);
        var sequence = go.GetComponent<BlockSequence>();
        sequences.Add(sequence);

        // Add block to sequence
        sequence.AddBlock(selectedBlock);
        blocksInSequences.Add(selectedBlock);

        // Make currently dragged block a child of the sequence container
        selectedBlock.transform.SetParent(sequence.transform);

        selectedBlock.OnDragged -= OnBlockFromSelectionBoxDragged;
        selectedBlock.OnDragged += OnBlockFromSequenceDragged;

        UpdateUI();
        Debug.Log($"Dropped {selectedBlock.name}!");
    }

    void UpdateUI()
    {
        foreach (var uiElement in uiToUpdate)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(uiElement);
        }
    }

}
