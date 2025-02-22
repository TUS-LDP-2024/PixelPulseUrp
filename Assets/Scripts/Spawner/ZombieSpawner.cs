using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    // List to store all spawners in the scene
    public static List<ZombieSpawner> AllSpawners = new List<ZombieSpawner>();
    private static List<ZombieSpawner> ActiveSpawners = new List<ZombieSpawner>(); // Track active spawners

    // Tracking zombie counts
    public static int zombiesAlive = 0;
    public static int zombiesSpawned = 0;
    public static int maxZombiesThisRound = 8; // First round starts with 8 zombies
    public static int currentRound = 1;
    public static float spawnIncreaseFactor = 1.2f; // Zombies increase per round

    // Spawner properties
    public GameObject zombiePrefab;
    public Transform spawnPoint;
    public float spawnInterval = 3f; // Time between spawns

    private static int spawnerIndex = 0; // Used to cycle through spawners in order
    private Coroutine spawnCoroutine; // Track the spawning coroutine

    private void Awake()
    {
        // Add this spawner to the global list
        if (!AllSpawners.Contains(this))
        {
            AllSpawners.Add(this);
        }

        // Disable the spawner at start
        this.enabled = false;
    }

    private void OnEnable()
    {
        // Add this spawner to the active spawners list
        if (!ActiveSpawners.Contains(this))
        {
            ActiveSpawners.Add(this);
            Debug.Log($"Added spawner to active list: {this.name}");
        }

        // Start the spawning logic if it's not already running
        if (spawnCoroutine == null && ActiveSpawners.Count > 0 && ActiveSpawners[0] == this)
        {
            StartNewRound();
        }
    }

    private void OnDisable()
    {
        // Remove this spawner from the active spawners list
        if (ActiveSpawners.Contains(this))
        {
            ActiveSpawners.Remove(this);
            Debug.Log($"Removed spawner from active list: {this.name}");
        }

        // Stop the coroutine if this spawner was running it
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private static void StartNewRound()
    {
        Debug.Log($"Starting Round {currentRound}");

        zombiesSpawned = 0;
        zombiesAlive = 0;

        // First round always has exactly 8 zombies
        if (currentRound == 1)
        {
            maxZombiesThisRound = 8;
        }
        else
        {
            // Increase zombie count for the next round
            maxZombiesThisRound = Mathf.RoundToInt(maxZombiesThisRound * spawnIncreaseFactor);
        }

        currentRound++; // Move to the next round
        spawnerIndex = 0; // Reset spawner order

        // Start staggered spawning using the first active spawner
        if (ActiveSpawners.Count > 0)
        {
            ActiveSpawners[0].spawnCoroutine = ActiveSpawners[0].StartCoroutine(StaggeredSpawn());
        }
    }

    private static IEnumerator StaggeredSpawn()
    {
        // Spawn zombies in a staggered order across all active spawners
        while (zombiesSpawned < maxZombiesThisRound)
        {
            if (ActiveSpawners.Count > 0)
            {
                ZombieSpawner currentSpawner = ActiveSpawners[spawnerIndex];

                // Ensure the spawner is active (enabled)
                if (currentSpawner.enabled)
                {
                    // Spawn a zombie if we haven't reached the max
                    if (zombiesSpawned < maxZombiesThisRound)
                    {
                        currentSpawner.SpawnZombie();
                    }

                    // Move to the next spawner in order
                    spawnerIndex = (spawnerIndex + 1) % ActiveSpawners.Count;
                }

                // Wait before the next spawner creates a zombie
                yield return new WaitForSeconds(currentSpawner.spawnInterval);
            }
        }

        Debug.Log($"All zombies spawned for Round {currentRound - 1}");
    }

    private void SpawnZombie()
    {
        // Prevent spawning more than allowed
        if (zombiesSpawned >= maxZombiesThisRound) return;

        if (zombiePrefab != null && spawnPoint != null)
        {
            // Create a new zombie at the spawner's location
            GameObject newZombie = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
            EnemyHealth enemyHealth = newZombie.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                // Subscribe to the zombie's death event
                enemyHealth.OnDeath += OnZombieDeath;
            }

            zombiesSpawned++;
            zombiesAlive++;

            Debug.Log($"Zombie spawned! Total spawned: {zombiesSpawned}, Alive: {zombiesAlive}");
        }
    }

    private void OnZombieDeath(GameObject zombie)
    {
        zombiesAlive--;

        Debug.Log($"Zombie died! Zombies alive: {zombiesAlive}");

        // When all zombies are dead and the round quota has been met, start a new round
        if (zombiesAlive <= 0 && zombiesSpawned >= maxZombiesThisRound)
        {
            Debug.Log("All zombies dead. Starting new round...");
            StartNewRound();
        }
    }
}