using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string itemCode = "";
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Hit");
        if (collision.gameObject.tag.Equals("Player"))
        {
            inventory.AddItem(itemCode);
            Destroy(gameObject);
        }
    }
}
