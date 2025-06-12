using GhostProgramming;
using System.Collections.Generic;

public class GhostNode : EntityNode<GhostController>
{
    protected override List<GhostController> GetEntityList()
    {
        return EntityManager.Instance.GetEntities<GhostController>();
    }

    protected override string GetEntityName()
    {
        return "Ghost";
    }
}
