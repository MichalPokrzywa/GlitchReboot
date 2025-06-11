using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab pocisku (wczytywany z Resources)
    public Transform firePoint; // Punkt wystrza�u
    public float launchForce = 10f; // Si�a wystrza�u

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Strza� po naci�ni�ciu spacji
        {
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        // Instancjonowanie pocisku
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * launchForce; // Nadajemy pr�dko��

        // Zniszczenie pocisku po 5 sekundach
        Destroy(projectile, 5f);
    }
}