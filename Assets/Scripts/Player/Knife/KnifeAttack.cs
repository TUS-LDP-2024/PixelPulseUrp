using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Needed for InputActionReference

public class KnifeAttack : MonoBehaviour
{
    [Header("Knife Attack Settings")]
    public float damage = 50f;           // Damage dealt to enemies
    public float attackRange = 1.5f;     // How far in front of the player the box extends
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);

    [Header("Cooldown Settings")]
    public float cooldownDuration = 1.5f;
    private bool canAttack = true;

    [Header("Input References")]
    public InputActionReference shootAction;
    // Assign your right-click (Aim) action in the Inspector

    void Update()
    {
        // If 'V' is pressed and we can attack, perform the knife attack
        if (Input.GetKeyDown(KeyCode.V) && canAttack)
        {
            PerformKnifeAttack();
        }
    }

    private void PerformKnifeAttack()
    {
        // Calculate the center of the box in front of the player
        Vector3 boxCenter = transform.position + transform.forward * attackRange;

        // OverlapBox takes half the box’s size, so we divide by 2
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize * 0.5f, transform.rotation);

        foreach (Collider hit in hits)
        {
            // Check if the hit object has EnemyHealth
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Knifed {hit.name} for {damage} damage.");
            }
        }

        // Start the cooldown
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        // Disable attack and right-click aim
        canAttack = false;
        if (shootAction != null && shootAction.action != null)
        {
            shootAction.action.Disable();
        }

        // Wait for the cooldown duration
        yield return new WaitForSeconds(cooldownDuration);

        // Re-enable aim and allow attacks again
        if (shootAction != null && shootAction.action != null)
        {
            shootAction.action.Enable();
        }
        canAttack = true;
    }

    // (Optional) Draw the box in the Scene view for easier tweaking
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + transform.forward * attackRange;
        // Temporarily adjust Gizmos’ matrix so the box rotates with the player
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
