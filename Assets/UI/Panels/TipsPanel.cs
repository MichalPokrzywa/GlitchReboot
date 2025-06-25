using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TipsPanel : Panel
{
    [SerializeField] TextMeshProUGUI text;

    private Dictionary<eTipType, (Func<InputSystem_Actions.PlayerActions, InputAction> actionSelector, string tipSuffix)> tipData;

    public enum eTipType
    {
        None,
        DiceThrow,
        DiceDrop,
        Tablet,
        TabletLanguage,
        Zoom,
        SpawnMarker,
        NextMarker,
        PrevMarker
    }

    void Awake()
    {
        tipData = new()
        {
            { eTipType.DiceThrow, (p => p.Fire, " to throw the dice") },
            { eTipType.DiceDrop, (p => p.Interact, " to drop the dice") },
            { eTipType.Tablet, (p => p.TabletInteract, " to open the tablet") },
            { eTipType.TabletLanguage, (p => p.Interact, " to change language in the terminal") },
            { eTipType.Zoom,  (p => p.Zoom, " to zoom — names will show up above objects that are used in the ghost programming panel.")},
            { eTipType.SpawnMarker, (p => p.SpawnMarker, " to spawn a marker") },
            { eTipType.NextMarker, (p => p.NextMarker, " to select next marker") },
            { eTipType.PrevMarker, (p => p.PrevMarker, " to select previous marker") }
        };
    }

    public void SetTips(params eTipType[] tips)
    {
        if (tips == null || tips.Length == 0 || (tips.Length == 1 && tips[0] == eTipType.None))
        {
            text.text = string.Empty;
            return;
        }

        var currentControls = InputManager.Instance.CurrentControls.Player;
        var combinedText = string.Empty;

        foreach (var tipType in tips)
        {
            if (tipType == eTipType.None)
                continue;

            if (!tipData.TryGetValue(tipType, out var tip))
            {
                Debug.LogWarning($"Tip type {tipType} not defined in TipData.");
                continue;
            }

            string binding = InputManager.Instance.GetBinding(tip.actionSelector(currentControls));
            combinedText += $"<b>{binding}</b>{tip.tipSuffix}\n";
        }

        text.text = combinedText.TrimEnd('\n');
    }

    public override void Close() => base.Close();

    public override void Open() => base.Open();
}