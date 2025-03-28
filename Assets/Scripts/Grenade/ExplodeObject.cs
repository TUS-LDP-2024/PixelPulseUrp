using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Damage Settings")]
    public float explosionDamage = 50f;      // Damage dealt by the explosion
    public float damageRadius = 5f;         // Radius for dealing damage to enemies

    [Header("Explosion Force Settings")]
    public float destroyDelay = 3f;         // Delay before child objects are destroyed
    public float minForce = 150f;           // Minimum explosion force for child pieces
    public float maxForce = 200f;           // Maximum explosion force for child pieces
    public float forceRadius = 500f;        // Radius for the physics explosion force on child pieces

    [Header("Smoke FX Settings")]
    public GameObject smoke;                // Smoke prefab to instantiate
    public int maximumSmokes = 30;          // Maximum number of smokes that can appear

    private bool hasCollided = false;

    void OnCollisionEnter(Collision collision)
    {
        // When colliding with an object tagged "Enemy" or "Ground", start the explosion timer.
        if (!hasCollided && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground")))
        {
            hasCollided = true;
            StartCoroutine(ExplosionTimer());
        }
    }

    // Wait 2 seconds after collision, then explode.
    IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(2f);
        Explode();
    }

    void Explode()
    {
        // 1) Deal area damage to any enemies within 'damageRadius'.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                // Assumes enemy objects have a "TakeDamage(float amount)" method
                hit.SendMessage("TakeDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // 2) Apply random explosion force to any child rigidbodies on the grenade object itself.
        //    (Typically used if your grenade has child pieces that break off.)
        int smokeCounter = 0;
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float randomForce = Random.Range(minForce, maxForce);
                rb.AddExplosionForce(randomForce, transform.position, forceRadius);
            }

            // 3) Randomly instantiate smoke on some child pieces (25% chance), limited by maximumSmokes.
            if (smoke != null && smokeCounter < maximumSmokes)
            {
                // 25% chance: Random.Range(1,4) == 1
                if (Random.Range(1, 4) == 1)
                {
                    GameObject smokeFX = Instantiate(smoke, child.position, Quaternion.identity);
                    smokeCounter++;
                    Destroy(smokeFX, 5f); // optional: remove smoke effect after 5 seconds
                }
            }

            // 4) Destroy each child piece after the specified delay.
            Destroy(child.gameObject, destroyDelay);
        }

        // Finally, destroy the grenade object itself immediately (or after the same delay if desired).
        Destroy(gameObject);
    }
}
