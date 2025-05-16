using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class DynamicNavMesh : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] float rebuildDelay = 1f;

    void Start()
    {
        StartCoroutine(UpdateNavMesh());
    }

    IEnumerator UpdateNavMesh()
    {
        while (true)
        {
            yield return new WaitForSeconds(rebuildDelay);
            var asyncOperation = navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
