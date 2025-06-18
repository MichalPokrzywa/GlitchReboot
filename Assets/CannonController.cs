using UnityEngine;

public class CannonController : InteractionBase
{
    public GameObject attachedObject;
    public GameObject player;
    public Transform firePoint; 
    public GameObject projectilePrefab;
    public float launchForce = 10f; 

    protected override void Start()
    {
        base.Start();
        TooltipText = "[E] " + "LOAD GUN";
    }

    public override void Update()
    {
        base .Update();
        // Debug
        Debug.DrawRay(firePoint.position, firePoint.forward * 5, Color.red, 2f);
        //if (Input.GetKeyDown(KeyCode.Space)) // Strzał po naciśnięciu spacji
        //{
        //    FireProjectile();
        //}
    }

    public override void Interact()
    {
        

        Debug.Log("Interakcja: E naciśnięte. Wpisz tutaj swoją logikę interakcji.");
        if(!player.GetComponent<Interactor>().IsHoldingObject())
        {
            FireProjectile();
        }


    }


    void FireProjectile()
    {
        //GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        //Rigidbody rb = projectile.GetComponent<Rigidbody>();
        //rb.AddForce(firePoint.forward * launchForce, ForceMode.Impulse);
        //Destroy(projectile, 5f);

        player.transform.position = firePoint.position;
        player.GetComponent<Rigidbody>().AddForce(firePoint.forward * launchForce * 10f, ForceMode.Impulse); // Nadajemy pr�dko��
    }
}