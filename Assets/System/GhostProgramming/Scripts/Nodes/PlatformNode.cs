using GhostProgramming;
using System.Collections.Generic;
using System.Linq;

public class PlatformNode : EntityNode<VariablePlatformBase>
{
    protected override List<VariablePlatformBase> GetEntityList()
    {
        var entities = EntityManager.Instance.GetEntities<VariablePlatformBase>();
        var currentPuzzle = TabletTerminal.Instance.assignedTerminal;

        if (currentPuzzle == null)
            return entities;

        var puzzleEntities = entities
            .Where(e => e.puzzleBase == currentPuzzle || e.puzzleBase == null)
            .ToList();
        return puzzleEntities;
    }
}
