using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GhostProgramming
{
    public class GoToActionNode : ActionNode
    {
        public override async Task<bool> Execute(CancellationToken cancelToken)
        {
            if (prevNode is not GhostNode ghostNode)
                return false;
            if (ghostNode.GetValue() is not GhostController ghost)
                return false;
            if (nextNode is not IEntityNode entityNode)
                return false;

            return await ghost.MoveTo(entityNode.GetEntity().gameObject, cancelToken);
        }

    }
}