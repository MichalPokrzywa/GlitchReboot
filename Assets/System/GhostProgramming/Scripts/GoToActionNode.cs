using UnityEngine;

namespace GhostProgramming
{
    public class GoToActionNode : ActionNode
    {
        public override bool Execute()
        {
            var ghostNode = prevNode as GhostNode;
            if (ghostNode == null)
                return false;

            var ghost = ghostNode.GetValue() as GhostController;
            if (ghost == null)
                return false;

            var targetNode = nextNode as ArgumentNode;
            if (targetNode == null)
                return false;

            var target = targetNode.GetValue() as MonoBehaviour;
            ghost.MoveTo(target?.gameObject);
            Debug.Log($"{ghost.name} go to {targetNode.name}");
            return true;
        }
    }
}