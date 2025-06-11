using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float gravity = -9.81f; // Si³a grawitacji
    private Vector3 velocity;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        velocity = rb.linearVelocity;
    }

    void Update()
    {
        velocity.y += gravity * Time.deltaTime; // Symulacja opadania
        transform.position += velocity * Time.deltaTime;
    }
}