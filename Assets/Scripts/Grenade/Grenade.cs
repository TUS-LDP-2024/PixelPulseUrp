using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float destroyDelay = 3f;       // Delay before destroying each child object.
    public float minForce = 150f;         // Minimum explosion force for child pieces.
    public float maxForce = 200f;         // Maximum explosion force for child pieces.
    public float explosionRadius = 5f;    // Radius in which enemies take damage.
    public float forceRadius = 5f;        // Radius for applying explosion force to child objects.
    public float explosionDelay = 2f;     // Delay (in seconds) after collision before explosion.

    [Header("Smoke FX Settings")]
    public GameObject smoke;              // Smoke prefab to instantiate.
    public int maximumSmokes = 30;        // Maximum number of smoke effects that can be spawned.

    [Header("Collision Settings")]
    // Tags that will trigger the explosion (e.g., "Enemy" or "Ground").
    public string[] explosionTags = { "Enemy", "Ground" };

    private bool hasCollided = false;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision object has any of the specified tags.
        foreach (string tag in explosionTags)
        {
            if (!hasCollided && collision.gameObject.CompareTag(tag))
            {
                hasCollided = true;
                StartCoroutine(ExplosionTimer());
                break;
            }
        }
    }

    IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    public void Explode()
    {
        // 1) Damage all enemies within explosionRadius by calling TakeRandomDamage().
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.SendMessage("TakeRandomDamage", SendMessageOptions.DontRequireReceiver);
            }
        }

        // 2) For each child piece, apply explosion force and spawn smoke FX.
        int smokeCounter = 0;
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float randomForce = Random.Range(minForce, maxForce);
                rb.AddExplosionForce(randomForce, transform.position, forceRadius);
            }

            if (smoke != null && smokeCounter < maximumSmokes)
            {
                // 25% chance to spawn smoke on this child.
                if (Random.Range(1, 4) == 1)
                {
                    GameObject smokeFX = Instantiate(smoke, child.position, Quaternion.identity);
                    smokeCounter++;
                    Destroy(smokeFX, 5f);
                }
            }

            // Destroy the child object after the specified delay.
            Destroy(child.gameObject, destroyDelay);
        }

        // Finally, destroy the grenade object itself.
        Destroy(gameObject);
    }
}
