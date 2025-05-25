using System.Threading.Tasks;
using UnityEngine;

namespace GhostProgramming
{
    public class PickUpActionNode : ActionNode
    {
        public override async Task<bool> Execute()
        {
            if (prevNode is not GhostNode ghostNode)
                return false;
            if (ghostNode.GetValue() is not GhostController ghost)
                return false;
            if (nextNode is not ArgumentNode targetNode)
                return false;

            var pickable = (targetNode.GetValue() as MonoBehaviour)
                ?.GetComponent<PickUpObjectInteraction>();

            if (pickable == null)
                return false;

            return await ghost.PickUp(pickable);
        }

    }
}