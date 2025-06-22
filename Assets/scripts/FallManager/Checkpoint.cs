using UnityEngine;

public class Checkpoint : BoxTrigger
{
    [Tooltip("Miejsce, do którego będzie cofany gracz po śmierci.")]
    public Transform checkpointTarget;

    private void Reset()
    {
        base.Reset();
        AutoCreateCheckpointTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")||other.gameObject.layer == LayerMask.NameToLayer("VariableDice")) //easter egg
        {
            FallManager.Instance.SetCheckpoint(checkpointTarget);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 1. Rysowanie pola BoxCollidera (checkpointa)
        Gizmos.color = new Color(0.6f, 1f, 0.6f, 0.3f);
        Collider col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
        Gizmos.matrix = Matrix4x4.identity;
        // 2. Rysowanie punktu docelowego (checkpointTarget)
        if (checkpointTarget != null)
        {
            // Gizmos.color = Color.cyan;
            // Gizmos.DrawSphere(checkpointTarget.position, 0.2f);

            // 3. Linia od checkpointa do punktu odrodzenia
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, checkpointTarget.position);

            // 4. Opcjonalny label
            Gizmos.color = Color.green;
            UnityEditor.Handles.Label(checkpointTarget.position + Vector3.up * 0.3f, "Checkpoint Target");
        }
    }
#endif  //UNITY_EDITOR

    private void AutoCreateCheckpointTarget()
    {
        if (checkpointTarget == null)
        {
            // Sprawdź, czy już istnieje dziecko o tej nazwie
            Transform existing = transform.Find("CheckpointTarget");
            if (existing != null)
            {
                checkpointTarget = existing;
            }
            else
            {
                GameObject target = new GameObject("CheckpointTarget");
                target.transform.SetParent(this.transform);
                target.transform.localPosition = Vector3.up * 1.5f;
                checkpointTarget = target.transform;
                target.AddComponent<WaypointGizmo>();
            }
        }
    }

}
