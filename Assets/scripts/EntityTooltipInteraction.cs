using UnityEngine;

public class EntityTooltipInteraction : InteractionBase
{
    protected override void Start()
    {
        base.Start();
    }

    public void UpdateTooltip(string text)
    {
        TooltipText = text;
    }
}
