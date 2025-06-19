using UnityEngine;

public class EntityTooltipInteraction : InteractionBase
{
    protected override void Start()
    {
        base.Start();
        floatDistanceX = 10f;
    }

    public void UpdateTooltip(string text)
    {
        TooltipText = text;
    }
}
