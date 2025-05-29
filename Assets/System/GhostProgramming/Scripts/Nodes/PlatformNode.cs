using GhostProgramming;
using System.Collections.Generic;

public class PlatformNode : EntityNode<VariablePlatform>
{
    protected override List<VariablePlatform> GetEntityList()
    {
        return EntityManager.instance.GetEntities<VariablePlatform>();
    }

    protected override string GetEntityName()
    {
        return "Platform";
    }
}
