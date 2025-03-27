using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject fastZombiePrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;

    private void Start()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.AllSpawners.Add(this);
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (ShouldSpawn())
            {
                yield return StartCoroutine(SpawnZombieWithDelay());
            }
            yield return null;
        }
    }

    private bool ShouldSpawn()
    {
        return RoundManager.Instance != null
            && RoundManager.Instance.IsRoundActive
            && RoundManager.Instance.CanSpawnMoreZombies();
    }

    private IEnumerator SpawnZombieWithDelay()
    {
        yield return new WaitForSeconds(spawnInterval);

        if (!ShouldSpawn()) yield break;

        GameObject prefabToSpawn = Random.value < 0.3f ? fastZombiePrefab : zombiePrefab;
        GameObject zombie = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

        if (zombie.TryGetComponent<EnemyHealth>(out var health))
        {
            RoundManager.Instance.IncrementSpawnedCount();
            health.OnDeath += HandleZombieDeath;
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