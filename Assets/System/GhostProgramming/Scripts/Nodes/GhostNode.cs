using GhostProgramming;
using System.Collections.Generic;

public class GhostNode : EntityNode<GhostController>
{
    protected override List<GhostController> GetEntityList()
    {
        return EntityManager.instance.ghosts;
    }

    protected override string GetEnityName()
    {
        return "Ghost";
    }
}
