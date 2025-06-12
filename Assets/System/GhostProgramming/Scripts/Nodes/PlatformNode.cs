using GhostProgramming;
using System.Collections.Generic;

public class PlatformNode : EntityNode<VariablePlatformBase>
{
    protected override List<VariablePlatformBase> GetEntityList()
    {
        return EntityManager.Instance.GetEntities<VariablePlatformBase>();
    }

    protected override string GetEntityName()
    {
        return "Platform";
    }
}
