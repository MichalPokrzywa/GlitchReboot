using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FallZone : BoxTrigger
{
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            FallManager.Instance.ResetPlayer(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("VariableDice"))
        {
            FallManager.Instance.ResetDice(other.gameObject);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // półprzezroczysty czerwony
        Collider col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}
