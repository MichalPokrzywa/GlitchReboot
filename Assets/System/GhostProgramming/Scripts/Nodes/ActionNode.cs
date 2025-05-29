
using System.Threading;
using System.Threading.Tasks;
using static SequenceRunner;

namespace GhostProgramming
{
    public abstract class ActionNode : Node
    {
        public abstract Task<bool> Execute(CancellationToken cancelToken, ExecutionResult result);
    }
}