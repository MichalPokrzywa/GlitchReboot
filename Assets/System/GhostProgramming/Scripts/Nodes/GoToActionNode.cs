using System.Threading;
using System.Threading.Tasks;
using static SequenceRunner;

namespace GhostProgramming
{
    public class GoToActionNode : ActionNode
    {
        public override async Task<bool> Execute(CancellationToken cancelToken, ExecutionResult result)
        {
            if (prevNode is not GhostNode ghostNode || ghostNode.GetValue() is not GhostController ghost)
            {
                result.errorCode = SequenceRunner.ErrorCode.NoGhostBeforeAction;
                return false;
            }

            if (nextNode is not IEntityNode entityNode)
            {
                result.errorCode = SequenceRunner.ErrorCode.NoObjectAfterAction;
                return false;
            }

            currentlyExecuting = prevNode.currentlyExecuting = nextNode.currentlyExecuting = true;
            var taskResult = await ghost.MoveTo(entityNode.GetEntity().gameObject, cancelToken, result);
            currentlyExecuting = prevNode.currentlyExecuting = nextNode.currentlyExecuting = false;
            return taskResult;
        }

    }
}