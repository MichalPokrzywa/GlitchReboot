using UnityEngine;

public class CannonController : InteractionBase
{
    public GameObject attachedObject;
    public GameObject player;
    public Transform firePoint; 
    public float launchForce = 10f; 

    protected override void Start()
    {
        base.Start();
        TooltipText = "[E] " + "LOAD GUN";
    }

    public override void Interact()
    {
        Debug.Log("Interakcja: E naciśnięte. Wpisz tutaj swoją logikę interakcji.");
        if(!player.GetComponent<Interactor>().IsHoldingObject())
        {
            player.GetComponent<Rigidbody>().useGravity = false;
            player.transform.position = firePoint.position;
            player.transform.rotation = firePoint.rotation;
            
        }
    }


    void FireProjectile()
    {
        // Instancjonowanie pocisku
        //Rigidbody rb = projectile.GetComponent<Rigidbody>();
        player.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * launchForce; // Nadajemy pr�dko��

        // Zniszczenie pocisku po 5 sekundach
    }
}