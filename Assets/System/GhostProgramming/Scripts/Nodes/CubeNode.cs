using GhostProgramming;
using System.Collections.Generic;
using System.Linq;

public class CubeNode : EntityNode<VariableDice>
{
    protected override List<VariableDice> GetEntityList()
    {
        return EntityManager.Instance.GetEntities<VariableDice>();
    }
}
