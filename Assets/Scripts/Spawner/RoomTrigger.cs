using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public List<ZombieSpawner> spawnersInThisRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable all spawners first
            foreach (var spawner in RoundManager.Instance.AllSpawners)
            {
                spawner.SetActivation(false);
            }

            // Enable only spawners in this room
            foreach (var spawner in spawnersInThisRoom)
            {
                if (spawner != null)
                {
                    spawner.SetActivation(true);
                }
            }

            Debug.Log($"Entered room with {spawnersInThisRoom.Count} active spawners");
        }
    }
}