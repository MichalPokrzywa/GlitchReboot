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
    [SerializeField] Button executeButton;
    [SerializeField] Button stopExecution;
    [SerializeField] TextMeshProUGUI infoLabel;
    [SerializeField] SequenceManager sequenceManager;

    const string noSelectedSequencesInfo = "You have to select sequences you want to run";
    const string incorrectBlockInSequenceInfo = "There is invalid node in the sequence";

    Dictionary<BlockSequence, CancellationTokenSource> runningSequences = new();

    void Awake()
    {
        executeButton.onClick.AddListener(OnExecuteClicked);
        stopExecution.onClick.AddListener(OnStopExecutionClicked);
        stopExecution.gameObject.SetActive(false);
    }

    void Update()
    {
        bool anySelectedRunning = sequenceManager.GetSelectedSequences()
            .Any(seq => runningSequences.ContainsKey(seq));

        stopExecution.gameObject.SetActive(anySelectedRunning);
    }

    void OnDestroy()
    {
        executeButton.onClick.RemoveListener(OnExecuteClicked);
        stopExecution.onClick.RemoveListener(OnStopExecutionClicked);
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

            sequenceTasks.Add(RunSequenceAsync(sequence, cts.Token)
                .ContinueWith(_ => runningSequences.Remove(sequence)));
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
                cancelToken.ThrowIfCancellationRequested();

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
        }
    }
}
