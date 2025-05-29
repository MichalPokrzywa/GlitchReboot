using System.Threading;
using System.Threading.Tasks;
using static SequenceRunner;

namespace GhostProgramming
{
    public class DropActionNode : ActionNode
    {
        public override async Task<bool> Execute(CancellationToken cancelToken, ExecutionResult result)
        {
            if (prevNode is not GhostNode ghostNode || ghostNode.GetValue() is not GhostController ghost)
            {
                result.errorCode = ErrorCode.NoGhostBeforeAction;
                return false;
            }

            return ghost.Drop(result);
        }
    }
}