using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GhostProgramming;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SequenceRunner : MonoBehaviour
{
    [SerializeField] Button runButton;
    [SerializeField] Button cancelButton;
    [SerializeField] TextMeshProUGUI infoLabel;
    [SerializeField] SequenceManager sequenceManager;

    const string noSelectedSequencesInfo = "You have to select sequences you want to run";
    const string incorrectBlockInSequenceInfo = "There is invalid node in the sequence";
    const string ghostRepeatInSequencesInfo = "A ghost cannot have multiple sequences running simultaneously";

    Dictionary<BlockSequence, CancellationTokenSource> runningSequences = new();

    void Awake()
    {
        runButton.onClick.AddListener(OnExecuteClicked);
        cancelButton.onClick.AddListener(OnStopExecutionClicked);
        cancelButton.gameObject.SetActive(false);
    }

    void Update()
    {
        bool anySelectedRunning = sequenceManager.GetSelectedSequences()
            .Any(seq => runningSequences.ContainsKey(seq));

        cancelButton.gameObject.SetActive(anySelectedRunning);
    }

    void OnDestroy()
    {
        runButton.onClick.RemoveListener(OnExecuteClicked);
        cancelButton.onClick.RemoveListener(OnStopExecutionClicked);
    }

    void OnStopExecutionClicked()
    {
        // stop only selected sequences
        var toCancel = new List<BlockSequence>();
        foreach (var pair in runningSequences)
        {
            if (pair.Key.IsSelected)
            {
                pair.Value.Cancel();
                toCancel.Add(pair.Key);
            }
        }
        foreach (var seq in toCancel)
        {
            runningSequences[seq].Dispose();
            runningSequences.Remove(seq);
        }
    }

    async void OnExecuteClicked()
    {
        infoLabel.text = string.Empty;

        var sequences = sequenceManager.GetSelectedSequences();
        if (sequences.Count == 0)
        {
            infoLabel.text = noSelectedSequencesInfo;
            return;
        }

        // don't allow running multiple sequences with the same ghost
        bool ghostsRepeat = IsSameGhostInManySequences(sequences);
        if (ghostsRepeat)
        {
            infoLabel.text = ghostRepeatInSequencesInfo;
            return;
        }

        List<Task> sequenceTasks = new List<Task>();

        foreach (var sequence in sequences)
        {
            // if the sequence is already running, skip it
            if (runningSequences.ContainsKey(sequence))
                continue;

            // if the sequence has invalid blocks, skip it
            if (sequence.Blocks.Any(b => b.BlockNode != null && b.BlockNode.isValid == false))
            {
                infoLabel.text = incorrectBlockInSequenceInfo;
                continue;
            }

            var cts = new CancellationTokenSource();
            runningSequences[sequence] = cts;

            sequenceTasks.Add(RunSequenceAsync(sequence, cts.Token));
        }

        await Task.WhenAll(sequenceTasks);
    }

    async Task RunSequenceAsync(BlockSequence sequence, CancellationToken cancelToken)
    {
        foreach (var block in sequence.Blocks)
        {
            block.BlockNode.isInRunningSequence = true;
        }

        try
        {
            foreach (var block in sequence.Blocks)
            {
                //cancelToken.ThrowIfCancellationRequested();

                var node = block.BlockNode;
                if (node == null)
                {
                    Debug.LogWarning($"Node is null for block: {block.BlockInfo.name}");
                    return;
                }

                if (node is ActionNode actionNode)
                    await actionNode.Execute(cancelToken);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"Sequence was canceled.");
        }
        finally
        {
            foreach (var block in sequence.Blocks)
            {
                block.BlockNode.isInRunningSequence = false;
            }
            if (runningSequences.TryGetValue(sequence, out var cts))
            {
                cts.Dispose();
                runningSequences.Remove(sequence);
            }
        }
    }

    bool IsSameGhostInManySequences(List<BlockSequence> sequences)
    {
        var ghostToSequences = new Dictionary<GhostController, HashSet<BlockSequence>>();

        foreach (var seq in sequences)
        {
            foreach (var block in seq.Blocks)
            {
                if (block.BlockNode is GhostNode ghostNode)
                {
                    var ghost = ghostNode.GetValue();
                    if (ghost == null)
                        continue;

                    if (!ghostToSequences.TryGetValue(ghost, out var seqSet))
                    {
                        seqSet = new HashSet<BlockSequence>();
                        ghostToSequences[ghost] = seqSet;
                    }

                    seqSet.Add(seq);
                }
            }
        }

        return ghostToSequences.Values.Any(set => set.Count > 1);
    }
}
