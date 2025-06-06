using GhostProgramming;
using System.Collections.Generic;

public class MarkerPointNode : EntityNode<MarkerScript>
{
    protected override List<MarkerScript> GetEntityList()
    {
        return EntityManager.instance.GetEntities<MarkerScript>();
    }

    protected override string GetEntityName()
    {
        return "Marker";
    }
}
