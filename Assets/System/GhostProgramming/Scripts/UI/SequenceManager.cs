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
    readonly List<Block> blocksInSelectionBox = new List<Block>();

    Block selectedBlock;

    void Awake()
    {
        InitBlocksFromSelectionBox();
    }

    #region InterfacesImplementation

    public void OnDrop(PointerEventData eventData)
    {
        // Create new sequence
        var go = Instantiate(sequencePrefab.gameObject, sequenceContainer.transform);
        var newSequence = go.GetComponent<BlockSequence>();
        newSequence.OnDropped += OnDrop;
        sequences.Add(newSequence);

        OnDrop(newSequence);
    }

    #endregion

    void OnDrop(BlockSequence sequence)
    {
        // 1) This block comes from the selection box
        if (selectedBlock.BlockInfo.parentType == Block.BlockParentType.SelectionBox)
        {
            OnDropFromSelectionBox(sequence);
        }

        // 2) This block comes from another sequence
        else
        {
            OnDropFromSequence(sequence);
        }

        // Turn on raycast target for all images in the block
        selectedBlock.RaycastTargetActivation(true);

        UpdateUI();
    }

    void OnDropFromSelectionBox(BlockSequence sequence)
    {
        // Add block to sequence
        sequence.AddBlock(selectedBlock);
        // Make currently dragged block a child of the sequence container
        selectedBlock.transform.SetParent(sequence.transform);
        // Update events
        selectedBlock.OnDragged -= OnBlockFromSelectionBoxDragged;
        selectedBlock.OnDragged += OnBlockFromSequenceDragged;
        selectedBlock.UpdateParentType(Block.BlockParentType.Sequence);
    }

    void OnDropFromSequence(BlockSequence sequence)
    {
        // Remove block from previous sequence and add to the new one
        BlockSequence previousSequence = sequences.Find(seq => seq.ContainsBlock(selectedBlock));
        if (previousSequence != null && previousSequence != sequence)
        {
            previousSequence.RemoveBlock(selectedBlock);
            sequence.AddBlock(selectedBlock);
        }
        // Make currently dragged block a child of the sequence container
        selectedBlock.transform.SetParent(sequence.transform);

        DestroyEmptySequence(previousSequence);
    }

    void OnBlockFromSelectionBoxDragged(Block block)
    {
        // Make currently dragged block a child of the temporary block placement
        // to display it on the top of all other UI elements
        block.transform.SetParent(temporaryBlockPlacement.transform);

        // Turn off raycast target for all images in the block
        block.RaycastTargetActivation(false);

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
    }

    void OnBlockFromSequenceDragged(Block block)
    {
        block.transform.SetParent(temporaryBlockPlacement.transform);
        block.RaycastTargetActivation(false);
        SelectBlock(block);

        // check if should be swapped with another block

        // check if should be removed from sequence

        // check if should be swapped with another sequence
    }

    void DestroyEmptySequence(BlockSequence sequence)
    {
        if (sequence != null && sequence.BlockCount == 0)
        {
            Destroy(sequence.gameObject);
        }
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

    void SelectBlock(Block block)
    {
        selectedBlock?.ToggleSelection();
        selectedBlock = block;
        selectedBlock.ToggleSelection();
    }
    void UpdateUI()
    {
        foreach (var uiElement in uiToUpdate)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(uiElement);
        }
    }
}
