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

    const string noSelectedSequenceInfo = "You have to select sequences you want to run";
    const string incorrectBlockInSequenceInfo = "There is invalid node in the sequence";
    const string ghostRepeatInSequencesInfo = "A ghost cannot have multiple sequences running simultaneously";
    const string noGhostBeforeActionInfo = "A ghost must appear before the action";
    const string noObjectAfterActionInfo = "A target must appear after the action";
    const string noActionInfo = "There are no actions in the sequence";
    const string targetUnreachableInfo = "404: Target not found... or just somewhere the ghost can't reach";
    const string canceledInfo = "Sequence was canceled";
    const string notPickableInfo = "The ghost can't pick this up";
    const string nothingToDropInfo = "The ghost is not holding anything to drop";

    const string successInfo = "Task done!";

    Dictionary<BlockSequence, CancellationTokenSource> runningSequences = new();
    ExecutionResult executionResult;

    public class ExecutionResult
    {
        public ErrorCode errorCode;
    }

    public enum ErrorCode
    {
        None,
        NoGhostBeforeAction,
        NoObjectAfterAction,
        NoAction,
        TargetUnreachable,
        Canceled,
        NotPickable,
        NothingToDrop,

        NoSelectedSequence,
        IncorrectBlockInSequence,
        GhostRepeatInSequences
    }

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
        executionResult = new ExecutionResult { errorCode = ErrorCode.None };

        var sequences = sequenceManager.GetSelectedSequences();
        if (sequences.Count == 0)
        {
            executionResult.errorCode = ErrorCode.NoSelectedSequence;
            DisplayErrorInfo();
            return;
        }

        // add already running sequences to check
        var seqToCheck = new List<BlockSequence>(sequences);
        seqToCheck.AddRange(runningSequences.Keys.Where(s => !sequences.Contains(s)));

        // don't allow running multiple sequences with the same ghost
        bool ghostsRepeat = IsSameGhostInManySequences(seqToCheck);
        if (ghostsRepeat)
        {
            executionResult.errorCode = ErrorCode.GhostRepeatInSequences;
            DisplayErrorInfo();
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
                executionResult.errorCode = ErrorCode.IncorrectBlockInSequence;
                DisplayErrorInfo();
                return;
            }

            var cts = new CancellationTokenSource();
            runningSequences[sequence] = cts;

            sequenceTasks.Add(RunSequenceAsync(sequence, cts.Token));
        }

        await Task.WhenAll(sequenceTasks);
        DisplayErrorInfo();
    }

    async Task RunSequenceAsync(BlockSequence sequence, CancellationToken cancelToken)
    {
        foreach (var block in sequence.Blocks)
        {
            block.BlockNode.isInRunningSequence = true;
        }

        int actionsCount = 0;

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
                {
                    actionsCount++;
                    await actionNode.Execute(cancelToken, executionResult);
                    if (executionResult.errorCode != ErrorCode.None)
                    {
                        Debug.LogWarning($"Execution error ({executionResult.errorCode}) — stopping further actions in this sequence.");
                        break;
                    }
                }
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

            if (actionsCount == 0)
            {
                executionResult.errorCode = ErrorCode.NoAction;
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

    void DisplayErrorInfo()
    {
        infoLabel.color = executionResult.errorCode == ErrorCode.None ? Color.green : Color.red;

        switch (executionResult.errorCode)
        {
            case ErrorCode.NoSelectedSequence:
                infoLabel.text = noSelectedSequenceInfo;
                break;
            case ErrorCode.IncorrectBlockInSequence:
                infoLabel.text = incorrectBlockInSequenceInfo;
                break;
            case ErrorCode.GhostRepeatInSequences:
                infoLabel.text = ghostRepeatInSequencesInfo;
                break;
            case ErrorCode.NoGhostBeforeAction:
                infoLabel.text = noGhostBeforeActionInfo;
                break;
            case ErrorCode.NoObjectAfterAction:
                infoLabel.text = noObjectAfterActionInfo;
                break;
            case ErrorCode.NoAction:
                infoLabel.text = noActionInfo;
                break;
            case ErrorCode.TargetUnreachable:
                infoLabel.text = targetUnreachableInfo;
                break;
            case ErrorCode.Canceled:
                infoLabel.text = canceledInfo;
                break;
            case ErrorCode.NotPickable:
                infoLabel.text = notPickableInfo;
                break;
            case ErrorCode.NothingToDrop:
                infoLabel.text = nothingToDropInfo;
                break;
            case ErrorCode.None:
                infoLabel.text = successInfo;
                break;
            default:
                infoLabel.text = string.Empty;
                break;
        }
    }
}
