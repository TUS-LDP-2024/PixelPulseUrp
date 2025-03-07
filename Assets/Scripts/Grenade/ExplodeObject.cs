using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeObject : MonoBehaviour
{
    public float destroyDelay;
    public float minForce;
    public float maxForce;
    public float radius;
    public GameObject smoke;
    public int maximumSmokes;

    void Start()
    {
        Explode();
    }
    public void Explode()
    {
        int smokeCounter = 0;

        foreach (Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);
            }

            if (smoke != null && smokeCounter < maximumSmokes)
            {
                if (Random.Range(1, 4) == 1)
                {
                    GameObject smokeFX = Instantiate(smoke, t.transform) as GameObject;
                    smokeCounter++;
                    Destroy(smokeFX, 5);
                }
            }
            Destroy(t.gameObject, destroyDelay);
        }
    }
}
