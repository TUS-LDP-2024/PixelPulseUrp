
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera; 
    private NavMeshAgent agent; 

    private RaycastHit[] hits = new RaycastHit[1];

    private void Awake()
    {
       
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); 

            
            if (Physics.RaycastNonAlloc(ray, hits) > 0)
            {
               
                agent.SetDestination(hits[0].point);
            }
        }
    }
}

