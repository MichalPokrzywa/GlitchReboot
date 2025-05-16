using System.Threading.Tasks;

namespace GhostProgramming
{
    public class DropActionNode : ActionNode
    {
        public override async Task<bool> Execute()
        {
            if (prevNode is not GhostNode ghostNode)
                return false;
            if (ghostNode.GetValue() is not GhostController ghost)
                return false;

            return ghost.Drop();
        }
    }
}