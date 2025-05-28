
using UnityEngine;

namespace GhostProgramming
{
    public abstract class ArgumentNode<T> : Node
    {
        public abstract T GetValue();
    }
}
