using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Assign the zombie prefab in the Inspector
    public float spawnInterval = 3f; // Time between spawns
    public int maxZombies = 20; // Maximum number of zombies to spawn
    private int zombiesSpawned = 0;

    void Start()
    {
        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        while (zombiesSpawned < maxZombies)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnZombie()
    {
        if (zombiePrefab != null)
        {
            Instantiate(zombiePrefab, transform.position, Quaternion.identity);
            zombiesSpawned++;
            Debug.Log("Zombies Spawned: " + zombiesSpawned);
        }
        else
        {
            Debug.LogError("Zombie Prefab is not assigned!");
        }
    }
}
