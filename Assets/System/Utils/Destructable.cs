using System;
using System.Collections;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField]
    private GameObject wholePrefab;
    [SerializeField] 
    private GameObject brokenPrefab;
    [SerializeField]
    private float ExplosiveForce = 1000;
    [SerializeField]
    private float ExplosiveRadius = 2;
    [SerializeField]
    private float PieceFadeSpeed = 0.25f;
    [SerializeField]
    private float PieceDestroyDelay = 5f;
    [SerializeField]
    private float PieceSleepCheckDelay = 0.1f;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("VariableDice"))
        {
            Explode(collision.gameObject.GetComponent<Rigidbody>());
        }
    }


    private void Explode(Rigidbody objectThrown)
    {
        //Destroy(Rigidbody);
        GetComponent<Collider>().enabled = false;
        wholePrefab.GetComponent<Renderer>().enabled = false;


        //ameObject brokenInstance = Instantiate(BrokenPrefab, transform.position, transform.rotation);
        wholePrefab.SetActive(true);
        brokenPrefab.SetActive(true);
        Rigidbody[] rigidbodies = brokenPrefab.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in rigidbodies)
        {
            if (objectThrown != null)
            {
                // inherit velocities
                body.linearVelocity = objectThrown.linearVelocity;
            }
            body.AddExplosionForce(ExplosiveForce, transform.position, ExplosiveRadius);
        }

        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }

    private IEnumerator FadeOutRigidBodies(Rigidbody[] Rigidbodies)
    {
        WaitForSeconds Wait = new WaitForSeconds(PieceSleepCheckDelay);
        float activeRigidbodies = Rigidbodies.Length;

        while (activeRigidbodies > 0)
        {
            yield return Wait;

            foreach (Rigidbody rigidbody in Rigidbodies)
            {
                if (rigidbody.IsSleeping())
                {
                    activeRigidbodies--;
                }
            }
        }


        yield return new WaitForSeconds(PieceDestroyDelay);

        float time = 0;
        Renderer[] renderers = Array.ConvertAll(Rigidbodies, GetRendererFromRigidbody);

        foreach (Rigidbody body in Rigidbodies)
        {
            Destroy(body.GetComponent<Collider>());
            Destroy(body);
        }

        while (time < 1)
        {
            float step = Time.deltaTime * PieceFadeSpeed;
            foreach (Renderer renderer in renderers)
            {
                renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
            }

            time += step;
            yield return null;
        }

        foreach (Renderer renderer in renderers)
        {
            Destroy(renderer.gameObject);
        }
        Destroy(gameObject);
    }

    private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody)
    {
        return Rigidbody.GetComponent<Renderer>();
    }
}
