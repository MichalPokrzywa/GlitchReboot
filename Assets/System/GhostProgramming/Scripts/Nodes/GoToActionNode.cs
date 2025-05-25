using System.Threading.Tasks;
using UnityEngine;

namespace GhostProgramming
{
    public class GoToActionNode : ActionNode
    {
        public override async Task<bool> Execute()
        {
            if (prevNode is not GhostNode ghostNode)
                return false;
            if (ghostNode.GetValue() is not GhostController ghost)
                return false;
            if (nextNode is not ArgumentNode targetNode)
                return false;

            var target = targetNode.GetValue() as MonoBehaviour;

            return await ghost.MoveTo(target?.gameObject);
        }

    }
}