using UnityEngine;

public class Checkpoint : BoxTrigger
{
    [Tooltip("Miejsce, do którego będzie cofany gracz po śmierci.")]
    public Transform checkpointTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")||other.gameObject.layer == LayerMask.NameToLayer("VariableDice")) //easter egg
        {
            FallManager.Instance.SetCheckpoint(checkpointTarget);
        }
    }
}
