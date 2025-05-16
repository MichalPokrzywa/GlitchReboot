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
        //[HideInInspector]
        public Node prevNode;
        //[HideInInspector]
        public Node nextNode;
    }
}
