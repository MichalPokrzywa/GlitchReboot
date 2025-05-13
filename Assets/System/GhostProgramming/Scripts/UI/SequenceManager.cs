using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SequenceManager : MonoBehaviour, IDropHandler
{
    [SerializeField] GameObject mainParent;
    [SerializeField] BlockSequence sequencePrefab;
    [SerializeField] GameObject sequenceContainer;
    [SerializeField] GameObject selectionBox;
    [SerializeField] GameObject temporaryBlockPlacement;
    [SerializeField] GameObject emptySpace;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] List<RectTransform> uiToUpdate;

    List<BlockSequence> sequences = new List<BlockSequence>();
    readonly List<Block> blocksInSelectionBox = new List<Block>();

    Block selectedBlock;
    Coroutine scrollCoroutine;

    void Awake()
    {
        InitBlocksFromSelectionBox();
        var noDropZones = mainParent.GetComponentsInChildren<NoDropZone>(true);
        foreach (var child in noDropZones)
        {
            child.OnDropped += OnNoDropZoneDropped;
        }
    }

    void OnDestroy()
    {
        var noDropZones = mainParent.GetComponentsInChildren<NoDropZone>(true);
        foreach (var child in noDropZones)
        {
            child.OnDropped -= OnNoDropZoneDropped;
        }
    }

    #region InterfacesImplementation

    public void OnDrop(PointerEventData pointerData)
    {
        if (selectedBlock == null)
            return;

        // Create new sequence
        var go = Instantiate(sequencePrefab.gameObject, sequenceContainer.transform);
        var newSequence = go.GetComponent<BlockSequence>();
        newSequence.OnDropped += OnDrop;
        sequences.Add(newSequence);

        OnDrop(newSequence, pointerData);
    }

    #endregion

    void OnDrop(BlockSequence sequence, PointerEventData pointerData)
    {
        if (selectedBlock == null)
            return;

        // 1) This block comes from the selection box
        if (selectedBlock.BlockInfo.parentType == Block.BlockParentType.SelectionBox)
        {
            OnDropFromSelectionBox(sequence, pointerData);
        }

        // 2) This block comes from another sequence
        else
        {
            OnDropFromSequence(sequence, pointerData);
        }

        // Turn on raycast target for all images in the block
        selectedBlock.RaycastTargetActivation(true);
        DeselectBlock();

        UpdateUI();
    }

    void OnDropFromSelectionBox(BlockSequence sequence, PointerEventData pointerData)
    {
        // Add block to sequence
        sequence.AddBlock(selectedBlock);
        // Make currently dragged block a child of the sequence container in correct position
        PlaceSelectedBlockInSequence(sequence, pointerData);
        // Update events
        selectedBlock.OnDragged -= OnBlockFromSelectionBoxDragged;
        selectedBlock.OnDragged += OnBlockFromSequenceDragged;
        selectedBlock.UpdateParentType(Block.BlockParentType.Sequence);
    }

    void OnDropFromSequence(BlockSequence sequence, PointerEventData pointerData)
    {
        // Remove block from previous sequence and add to the new one
        BlockSequence previousSequence = sequences.Find(seq => seq.ContainsBlock(selectedBlock));
        if (previousSequence != null && previousSequence != sequence)
            previousSequence.RemoveBlock(selectedBlock);

        if (!sequence.ContainsBlock(selectedBlock))
            sequence.AddBlock(selectedBlock);

        // Make currently dragged block a child of the sequence container in correct position
        PlaceSelectedBlockInSequence(sequence, pointerData);
        // Destroy the previous sequence if it is empty
        DestroyEmptySequence(previousSequence);
    }

    void OnNoDropZoneDropped()
    {
        if (selectedBlock == null)
            return;

        BlockSequence sequence = sequences.Find(seq => seq.ContainsBlock(selectedBlock));
        if (sequence != null)
        {
            sequence.RemoveBlock(selectedBlock);
        }

        Destroy(selectedBlock.gameObject);
        DeselectBlock();
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
    }

    void PlaceSelectedBlockInSequence(BlockSequence sequence, PointerEventData pointerData)
    {
        selectedBlock.transform.SetParent(sequence.transform);
        int insertIndex = GetIndexInSequence(sequence, pointerData);
        selectedBlock.transform.SetSiblingIndex(insertIndex);
        emptySpace.transform.SetAsLastSibling();

        // if sequence has the last index (including empty space), scroll to the bottom
        int sequenceIndex = sequence.transform.GetSiblingIndex();
        if (sequenceIndex == sequenceContainer.transform.childCount - 2)
        {
            scrollRect.verticalNormalizedPosition = 0.02f;

            if (scrollCoroutine == null)
            {
                if (scrollCoroutine != null)
                    StopCoroutine(scrollCoroutine);

                scrollCoroutine = StartCoroutine(SmoothScrollTo(0.25f, 0.02f));
            }
        }
    }

    int GetIndexInSequence(BlockSequence sequence, PointerEventData pointerData)
    {
        // Calculate the position of the block in the new sequence according to the drop position
        // Default to end
        int insertIndex = sequence.transform.childCount;
        for (int i = 0; i < sequence.transform.childCount; i++)
        {
            RectTransform child = sequence.transform.GetChild(i) as RectTransform;
            // Skip self if already present
            if (child == selectedBlock.transform)
                continue;

            Vector3 worldPos = child.position;
            if (pointerData.position.x < worldPos.x)
            {
                insertIndex = i;
                break;
            }
        }
        return insertIndex;
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
        selectedBlock?.UpdateSelection(false);
        selectedBlock = block;
        selectedBlock.UpdateSelection(true);
    }

    void DeselectBlock()
    {
        if (selectedBlock != null)
        {
            selectedBlock.UpdateSelection(false);
            selectedBlock = null;
        }
    }

    void UpdateUI()
    {
        foreach (var uiElement in uiToUpdate)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(uiElement);
        }
    }

    IEnumerator SmoothScrollTo(float duration = 0.25f, float targetPos = 0.02f)
    {
        yield return new WaitForEndOfFrame();

        float elapsedTime = 0f;
        float startPos = scrollRect.verticalNormalizedPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, targetPos, t);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = targetPos;
        scrollCoroutine = null;
    }
}
