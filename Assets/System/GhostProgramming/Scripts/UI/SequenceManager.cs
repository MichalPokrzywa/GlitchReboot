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
        var noDropZones = mainParent.GetComponentsInChildren<DropTriggerZone>(true);
        foreach (var child in noDropZones)
        {
            child.OnDropped += DiscardSelectedBlock;
        }
    }

    void OnDestroy()
    {
        var noDropZones = mainParent.GetComponentsInChildren<DropTriggerZone>(true);
        foreach (var child in noDropZones)
        {
            child.OnDropped -= DiscardSelectedBlock;
        }
    }

    public List<BlockSequence> GetSelectedSequences()
    {
        List<BlockSequence> selectedSequences = new List<BlockSequence>();
        foreach (var sequence in sequences)
        {
            if (sequence != null && sequence.IsSelected)
            {
                selectedSequences.Add(sequence);
            }
        }
        return selectedSequences;
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

        // If the sequence is running, discard the selected block
        if (sequence.IsRunning)
        {
            DiscardSelectedBlock();
            return;
        }

        SelectionBoxRaycastActivate();

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

        DeselectBlock();

        UpdateUI();
    }

    void OnDropFromSelectionBox(BlockSequence sequence, PointerEventData pointerData)
    {
        // Add block to sequence
        int insertIndex = GetIndexInSequence(sequence, pointerData);
        sequence.AddBlock(insertIndex, selectedBlock);
        // Make currently dragged block a child of the sequence container in correct position
        PlaceSelectedBlockInSequence(sequence, insertIndex);
        // Update events
        UnregisterFromSelectionBoxEvents(selectedBlock);
        RegisterToSequenceEvents(selectedBlock);
        selectedBlock.UpdateParentType(Block.BlockParentType.Sequence);
    }

    void OnDropFromSequence(BlockSequence sequence, PointerEventData pointerData)
    {
        // Remove block from previous sequence and add to the new one
        BlockSequence previousSequence = sequences.Find(seq => seq.ContainsBlock(selectedBlock));
        if (previousSequence != null && previousSequence != sequence)
            previousSequence.RemoveBlock(selectedBlock);

        int insertIndex = GetIndexInSequence(sequence, pointerData);

        if (sequence.ContainsBlock(selectedBlock))
            sequence.RemoveBlock(selectedBlock);

        sequence.AddBlock(insertIndex, selectedBlock);

        // Make currently dragged block a child of the sequence container in correct position
        PlaceSelectedBlockInSequence(sequence, insertIndex);
        // Destroy the previous sequence if it is empty
        DestroyEmptySequence(previousSequence);
    }

    public void DiscardSelectedBlock()
    {
        if (selectedBlock == null)
            return;

        SelectionBoxRaycastActivate();

        BlockSequence sequence = sequences.Find(seq => seq.ContainsBlock(selectedBlock));
        if (sequence != null)
        {
            sequence.RemoveBlock(selectedBlock);
        }

        Destroy(selectedBlock.gameObject);
        DeselectBlock();
        DestroyEmptySequence(sequence);
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
        blockCopy.name = block.name;

        // Swap the original block with the copy in the list
        Block blockCopyComponent = blockCopy.GetComponent<Block>();
        blocksInSelectionBox[index] = blockCopyComponent;
        blockCopyComponent.UpdateInteractability(false);
        RegisterToSelectionBoxEvents(blockCopyComponent);

        SelectBlock(block);
    }

    void OnBlockFromSequenceDragged(Block block)
    {
        block.transform.SetParent(temporaryBlockPlacement.transform);
        SelectBlock(block);
    }

    void PlaceSelectedBlockInSequence(BlockSequence sequence, int index)
    {
        selectedBlock.transform.SetParent(sequence.transform);
        selectedBlock.transform.SetSiblingIndex(index);
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
        int insertIndex = sequence.transform.childCount;

        RectTransform sequenceRect = sequence.GetComponent<RectTransform>();
        Camera eventCamera = pointerData.enterEventCamera;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(sequenceRect, pointerData.position, eventCamera, out localPoint))
            return insertIndex;

        for (int i = 0; i < sequence.transform.childCount; i++)
        {
            RectTransform child = sequence.transform.GetChild(i) as RectTransform;

            if (child == selectedBlock.transform)
                continue;

            Vector3 childLocalPos = sequenceRect.InverseTransformPoint(child.position);
            if (localPoint.x < childLocalPos.x)
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
                RegisterToSelectionBoxEvents(block);
            }
        }
    }

    void SelectionBoxRaycastActivate(Block block = null)
    {
        foreach (var b in blocksInSelectionBox)
        {
            b.UpdateInteractability(true);
        }
    }

    void SelectionBoxRaycastDeactivate(Block block = null)
    {
        foreach (var b in blocksInSelectionBox)
        {
            b.UpdateInteractability(false);
        }
    }

    void RegisterToSelectionBoxEvents(Block block)
    {
        block.OnDragged += OnBlockFromSelectionBoxDragged;
        block.OnDragged += SelectionBoxRaycastDeactivate;
    }

    void UnregisterFromSelectionBoxEvents(Block block)
    {
        block.OnDragged -= OnBlockFromSelectionBoxDragged;
        block.OnDragged -= SelectionBoxRaycastDeactivate;
    }

    void RegisterToSequenceEvents(Block block)
    {
        block.OnDragged += OnBlockFromSequenceDragged;
        block.OnDragged += SelectionBoxRaycastDeactivate;
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
