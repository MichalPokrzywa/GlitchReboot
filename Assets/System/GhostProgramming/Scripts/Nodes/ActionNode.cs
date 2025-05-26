
using System.Threading;
using System.Threading.Tasks;

namespace GhostProgramming
{
    public abstract class ActionNode : Node
    {
        public abstract Task<bool> Execute(CancellationToken cancelToken);
    }
}