using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<string> items = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddItem(string item)
    {
        items.Add(item);
    }

    public void RemoveItem(string item)
    {
        items.Remove(item);
    }

    public bool HasItem(string item)
    { 
        return items.Contains(item);
    }
}
