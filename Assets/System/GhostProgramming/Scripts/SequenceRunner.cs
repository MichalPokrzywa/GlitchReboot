using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostProgramming;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SequenceRunner : MonoBehaviour
{
    [SerializeField] Button executeButton;
    [SerializeField] TextMeshProUGUI infoLabel;
    [SerializeField] SequenceManager sequenceManager;

    const string noSelectedSequencesInfo = "You have to select sequences you want to run";

    void Awake()
    {
        executeButton.onClick.AddListener(OnExecuteClicked);
    }

    void OnDestroy()
    {
        executeButton.onClick.RemoveListener(OnExecuteClicked);
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
            sequenceTasks.Add(RunSequenceAsync(sequence));
        }

        await Task.WhenAll(sequenceTasks);
    }

    private async Task RunSequenceAsync(BlockSequence sequence)
    {
        foreach (var block in sequence.Blocks)
        {
            var node = block.BlockNode;
            if (node == null)
            {
                Debug.LogWarning($"Node is null for block: {block.BlockInfo.name}");
                return;
            }

            if (node is ActionNode actionNode)
                await actionNode.Execute();
        }
    }
}
