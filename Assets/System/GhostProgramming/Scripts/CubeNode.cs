using GhostProgramming;
using System.Collections.Generic;
using UnityEngine;

public class CubeNode : EntityNode<VariableDice>
{
    protected override List<VariableDice> GetEntityList()
    {
        return EntityManager.instance.cubes;
    }

    protected override string GetEnityName()
    {
        return "Cube";
    }
}
