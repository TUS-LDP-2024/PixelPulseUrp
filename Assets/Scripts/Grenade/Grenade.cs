using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float destroyDelay = 3f;        // Delay before destroying each child object.
    public float minForce = 150f;          // Minimum explosion force for child pieces.
    public float maxForce = 200f;          // Maximum explosion force for child pieces.
    public float explosionRadius = 5f;     // Radius in which enemies take damage.
    public float forceRadius = 5f;         // Radius for applying explosion force.
    public float explosionDelay = 2f;      // Delay after collision before explosion.

    [Header("Smoke FX Settings")]
    public GameObject smoke;
    public int maximumSmokes = 30;

    [Header("Collision Settings")]
    public string[] explosionTags = { "Enemy", "Floor" };

    private bool hasCollided = false;

    void OnCollisionEnter(Collision collision)
    {
        // Make sure we only trigger once
        if (hasCollided) return;

        // If the collided object has a matching tag, start the explosion timer
        foreach (string tag in explosionTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                Debug.Log("Grenade collided with: " + collision.gameObject.name);
                hasCollided = true;
                StartCoroutine(ExplosionTimer());
                break;
            }
        }
    }

    private IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    public void Explode()
    {
        // 1) Damage enemies within explosionRadius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.SendMessage("TakeRandomDamage", SendMessageOptions.DontRequireReceiver);
            }
        }

        // 2) Child pieces: apply explosion force & optionally spawn smoke
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
                // 25% chance to spawn smoke on this child
                if (Random.Range(1, 5) == 1)
                {
                    GameObject smokeFX = Instantiate(smoke, child.position, Quaternion.identity);
                    smokeCounter++;
                    Destroy(smokeFX, 5f);
                }
            }

            // Destroy child pieces after a delay
            Destroy(child.gameObject, destroyDelay);
        }

        // Finally, destroy the grenade
        Destroy(gameObject);
    }
}
