using System;
using UnityEngine;

public enum FallZoneTarget
{
    Player,
    Dice,
    Both
}


[RequireComponent(typeof(Collider))]
public class FallZone : BoxTrigger
{
    [Tooltip("Co ten FallZone łapie?")]
    public FallZoneTarget targetTypes = FallZoneTarget.Player | FallZoneTarget.Dice;

    private void OnTriggerEnter(Collider other)
    {
        if ((targetTypes == FallZoneTarget.Player || targetTypes == FallZoneTarget.Both) && other.CompareTag("Player"))
        {
            FallManager.Instance.ResetPlayer(other.gameObject);
        }

        if ((targetTypes == FallZoneTarget.Dice || targetTypes == FallZoneTarget.Both) && other.gameObject.layer == LayerMask.NameToLayer("VariableDice"))
        {
            FallManager.Instance.ResetDice(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        // Dobierz kolor na podstawie typów
        Gizmos.color = GetColorForTargetTypes(targetTypes);

        Collider col = GetComponent<Collider>();
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }

        // Resetuj macierz, żeby inne gizmy się nie zepsuły
        Gizmos.matrix = Matrix4x4.identity;
    }
    
    private Color GetColorForTargetTypes(FallZoneTarget types)
    {
        switch (types)
        {
            case FallZoneTarget.Player:
                return new Color(0.3f, 0.6f, 1f, 0.25f); // niebieski
            case FallZoneTarget.Dice:
                return new Color(1f, 0.3f, 0.3f, 0.25f); // czerwony
            case FallZoneTarget.Both:
                return new Color(0.8f, 0.3f, 1f, 0.25f); // fioletowy
            default:
                return new Color(0.3f, 0.3f, 0.3f, 0.25f); // szary – nic nie łapie
        }
    }

}
