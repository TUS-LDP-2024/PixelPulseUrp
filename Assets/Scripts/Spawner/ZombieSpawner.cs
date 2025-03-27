using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject fastZombiePrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;

    private bool isActive = false;
    private Coroutine spawnCoroutine;

    public void SetActivation(bool active)
    {
        isActive = active;

        if (active)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
            Debug.Log($"Activated spawner at {transform.position}");
        }
        else
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (CanSpawn())
            {
                yield return StartCoroutine(SpawnZombieWithDelay());
            }
            yield return null;
        }
    }

    private bool CanSpawn()
    {
        return isActive &&
               RoundManager.Instance != null &&
               RoundManager.Instance.IsRoundActive &&
               RoundManager.Instance.CanSpawnMoreZombies();
    }

    private IEnumerator SpawnZombieWithDelay()
    {
        yield return new WaitForSeconds(spawnInterval);

        if (!CanSpawn()) yield break;

        GameObject prefabToSpawn = Random.value < 0.3f ? fastZombiePrefab : zombiePrefab;
        GameObject zombie = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

        if (zombie.TryGetComponent<EnemyHealth>(out var health))
        {
            RoundManager.Instance.IncrementSpawnedCount();
            health.OnDeath += HandleZombieDeath;
            Debug.Log($"Spawned zombie at {transform.position}");
        }
    }

    private void HandleZombieDeath(GameObject zombie)
    {
        if (zombie.TryGetComponent<EnemyHealth>(out var health))
        {
            health.OnDeath -= HandleZombieDeath;
        }
        RoundManager.Instance.DecrementAliveCount();
    }
}