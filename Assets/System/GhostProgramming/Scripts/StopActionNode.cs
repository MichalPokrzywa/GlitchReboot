using UnityEngine;

namespace GhostProgramming
{
    public class StopActionNode : ActionNode
    {
        public override bool Execute()
        {
            var ghostNode = prevNode as GhostNode;
            if (ghostNode == null)
                return false;

            var ghost = ghostNode.GetValue() as GhostController;
            if (ghost == null)
                return false;

            ghost.Stop();
            Debug.Log($"{ghost.name} stopped");
            return true;
        }
    }
}