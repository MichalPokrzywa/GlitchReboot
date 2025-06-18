using UnityEngine;

namespace GhostProgramming
{
    /// <summary>
    /// Base class for all nodes in the ghost programming system.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]

    public abstract class Node : MonoBehaviour
    {
        public Node prevNode;
        public Node nextNode;
        public bool isValid = true;
        public bool isInRunningSequence = false;
        public bool currentlyExecuting = false;
    }
}
