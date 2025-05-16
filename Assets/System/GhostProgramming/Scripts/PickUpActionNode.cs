using UnityEngine;

namespace GhostProgramming
{
    public class PickUpActionNode : ActionNode
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

            var value = targetNode.GetValue() as MonoBehaviour;
            var pickable = value?.gameObject.GetComponent<PickUpObjectInteraction>();
            if (pickable == null)
                return false;

            ghost.PickUp(pickable);
            Debug.Log($"{ghost.name} picking up {targetNode.name}");
            return true;
        }
    }
}