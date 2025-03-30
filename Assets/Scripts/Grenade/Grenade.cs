using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionDelay = 2f;     // Delay after trigger before explosion.
    public float explosionRadius = 5f;    // Radius for applying explosion damage/effects.
    public float forceRadius = 5f;        // Radius for explosion force.
    public float minForce = 150f;         // Minimum force for fragments.
    public float maxForce = 200f;         // Maximum force for fragments.
    public float destroyDelay = 3f;       // Time after explosion to destroy the grenade.

    [Header("FX Settings")]
    public ParticleSystem vfx_Explosion;  // Explosion effect (ensure it's not set to Play On Awake)

    [Header("Collision Settings")]
    public string[] explosionTags = { "Enemy", "Floor" };  // Adjust these tags as needed

    private bool hasTriggered = false;

    // Use OnTriggerEnter because the grenade's collider is set as a trigger.
    void OnTriggerEnter(Collider other)
    {
        foreach (string tag in explosionTags)
        {
            if (!hasTriggered && other.gameObject.CompareTag(tag))
            {
                Debug.Log("Grenade triggered by " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ")");
                hasTriggered = true;
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
        // Play the explosion effect.
        if (vfx_Explosion != null)
        {
            vfx_Explosion.Play();
            // Optional: Detach the effect so it persists even after the grenade is destroyed.
            // vfx_Explosion.transform.parent = null;
        }

        // Enable physics on child fragments by setting them to non-kinematic.
        foreach (Transform child in transform)
        {
            Rigidbody childRb = child.GetComponent<Rigidbody>();
            if (childRb != null)
            {
                childRb.isKinematic = false;
                float randomForce = Random.Range(minForce, maxForce);
                childRb.AddExplosionForce(randomForce, transform.position, forceRadius);
            }
        }

        // Optionally, damage nearby enemies.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.SendMessage("TakeRandomDamage", SendMessageOptions.DontRequireReceiver);
            }
        }

        // Destroy the grenade after a short delay.
        Destroy(gameObject, destroyDelay);
    }
}
