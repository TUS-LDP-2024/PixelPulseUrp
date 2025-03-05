using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoRestored = 20;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerCollider")
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var playerShooting = player.GetComponent<PlayerShooting>();
            playerShooting.storedAmmo = Math.Min(playerShooting.maxStoredAmmo, playerShooting.storedAmmo + ammoRestored);
            playerShooting.UpdateAmmoDisplay();
            Destroy(this.gameObject);
        }

    }
}