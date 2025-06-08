using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class FilterNavMeshSurface : MonoBehaviour
{
    public LayerMask layerMask;

    void Start()
    {
        var surface = GetComponent<NavMeshSurface>();
        surface.layerMask = layerMask;
        surface.BuildNavMesh();
    }
}