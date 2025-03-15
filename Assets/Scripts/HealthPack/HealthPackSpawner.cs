using UnityEngine;
using System.Collections;

public class HealthPackSpawner : MonoBehaviour
{
    // The prefab for the health pack.
    public GameObject healthPackPrefab;

    // The fixed spawn location (set via the Inspector).
    public Transform spawnPoint;

    // Spawn interval in seconds (e.g., 60 or 90 seconds).
    public float spawnInterval = 90f;

    // Reference to the currently spawned health pack.
    private GameObject currentHealthPack;

    private void Start()
    {
        StartCoroutine(SpawnHealthPackRoutine());
    }

    private IEnumerator SpawnHealthPackRoutine()
    {
        while (true)
        {
            // If there isn't an active health pack, spawn one.
            if (currentHealthPack == null)
            {
                currentHealthPack = Instantiate(healthPackPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            // Wait until the current health pack is used (destroyed).
            yield return new WaitUntil(() => currentHealthPack == null);

            // Then wait for the defined interval before spawning the next one.
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
