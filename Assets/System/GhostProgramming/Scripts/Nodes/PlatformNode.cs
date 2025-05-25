using GhostProgramming;
using System.Collections.Generic;

public class PlatformNode : EntityNode<VariablePlatform>
{
    protected override List<VariablePlatform> GetEntityList()
    {
        return EntityManager.instance.platforms;
    }

    protected override string GetEnityName()
    {
        return "Platform";
    }
}
