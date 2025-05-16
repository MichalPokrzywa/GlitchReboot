using GhostProgramming;
using NUnit.Framework;
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

    void OnExecuteClicked()
    {
        infoLabel.text = string.Empty;

        var sequences = sequenceManager.GetSelectedSequences();

        if (sequences.Count == 0)
        {
            infoLabel.text = noSelectedSequencesInfo;
            return;
        }

        foreach (var sequence in sequences)
        {
            foreach (var block in sequence.Blocks)
            {
                var node = block.BlockNode;
                if (node == null)
                {
                    Debug.LogWarning($"Node is null for block: {block.BlockInfo.name}");
                    return;
                }

                var actionNode = node as ActionNode;
                if (actionNode != null)
                    actionNode.Execute();
            }
        }
    }
}
