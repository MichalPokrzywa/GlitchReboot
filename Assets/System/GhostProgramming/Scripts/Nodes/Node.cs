using Unity.Collections;
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
        [ReadOnly]
        public Node prevNode;
        [ReadOnly]
        public Node nextNode;
    }
}
