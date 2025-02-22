using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    // List of spawner GameObjects associated with this room
    public List<GameObject> spawnersInRoom;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered room: {gameObject.name}");

            // Disable all spawners in the scene first
            DisableAllSpawners();

            // Enable spawners in this room
            EnableSpawnersInRoom();
        }
    }

    private void DisableAllSpawners()
    {
        // Loop through all spawners in the scene and disable their scripts
        foreach (ZombieSpawner spawner in ZombieSpawner.AllSpawners)
        {
            if (spawner != null)
            {
                spawner.enabled = false; // Disable the script
                Debug.Log($"Disabled spawner script: {spawner.name}");
            }
        }
    }

    private void EnableSpawnersInRoom()
    {
        // Enable spawners associated with this room
        foreach (GameObject spawnerObject in spawnersInRoom)
        {
            if (spawnerObject != null)
            {
                ZombieSpawner spawner = spawnerObject.GetComponent<ZombieSpawner>();
                if (spawner != null)
                {
                    spawner.enabled = true; // Enable the script
                    Debug.Log($"Enabled spawner script: {spawner.name}");
                }
            }
        }
    }
}