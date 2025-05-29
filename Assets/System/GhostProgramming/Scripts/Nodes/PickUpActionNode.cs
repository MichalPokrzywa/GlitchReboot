using System.Threading;
using System.Threading.Tasks;
using static SequenceRunner;

namespace GhostProgramming
{
    public class PickUpActionNode : ActionNode
    {
        public override async Task<bool> Execute(CancellationToken cancelToken, ExecutionResult result)
        {
            if (prevNode is not GhostNode ghostNode || ghostNode.GetValue() is not GhostController ghost)
            {
                result.errorCode = ErrorCode.NoGhostBeforeAction;
                return false;
            }

            if (nextNode is not IEntityNode entityNode)
            {
                result.errorCode = ErrorCode.NoObjectAfterAction;
                return false;
            }

            var pickable = entityNode.GetEntity().GetComponent<PickUpObjectInteraction>();
            if (pickable == null)
            {
                result.errorCode = ErrorCode.NotPickable;
                return false;
            }

            currentlyExecuting = prevNode.currentlyExecuting = nextNode.currentlyExecuting = true;
            var taskResult =  await ghost.PickUp(pickable, cancelToken, result);
            currentlyExecuting = prevNode.currentlyExecuting = nextNode.currentlyExecuting = false;
            return taskResult;
        }

    }
}