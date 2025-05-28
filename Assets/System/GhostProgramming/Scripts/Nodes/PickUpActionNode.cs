using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GhostProgramming
{
    public class PickUpActionNode : ActionNode
    {
        public override async Task<bool> Execute(CancellationToken cancelToken)
        {
            if (prevNode is not GhostNode ghostNode)
                return false;
            if (ghostNode.GetValue() is not GhostController ghost)
                return false;
            if (nextNode is not IEntityNode entityNode)
                return false;

            var pickable = entityNode.GetEntity().GetComponent<PickUpObjectInteraction>();
            if (pickable == null)
                return false;

            return await ghost.PickUp(pickable, cancelToken);
        }

    }
}