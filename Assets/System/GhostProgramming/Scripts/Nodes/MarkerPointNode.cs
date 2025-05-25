using GhostProgramming;
using System.Collections.Generic;

public class MarkerPointNode : EntityNode<MarkerScript>
{
    protected override List<MarkerScript> GetEntityList()
    {
        return EntityManager.instance.markers;
    }

    protected override string GetEnityName()
    {
        return "Marker";
    }
}
