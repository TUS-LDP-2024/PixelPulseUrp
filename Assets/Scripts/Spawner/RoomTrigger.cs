using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public List<GameObject> spawnersInRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DisableAllSpawners();
            EnableSpawnersInRoom();
        }
    }

    private void DisableAllSpawners()
    {
        // Use the AllSpawners property from RoundManager
        foreach (ZombieSpawner spawner in RoundManager.Instance.AllSpawners)
        {
            if (spawner != null)
            {
                spawner.enabled = false;
            }
        }
    }

    private void EnableSpawnersInRoom()
    {
        foreach (GameObject spawnerObject in spawnersInRoom)
        {
            if (spawnerObject != null)
            {
                ZombieSpawner spawner = spawnerObject.GetComponent<ZombieSpawner>();
                if (spawner != null)
                {
                    spawner.enabled = true;
                }
            }
        }
    }
}