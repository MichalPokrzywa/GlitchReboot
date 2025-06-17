using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    private void Reset()
    {
        // Ustaw collider jako trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        RemoveRendererIfExists();
    }
    private void RemoveRendererIfExists()
    {
#if UNITY_EDITOR
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            DestroyImmediate(rend);
        }
#endif
    }
}
