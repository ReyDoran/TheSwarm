using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmPerception : MonoBehaviour
{
    private Swarm mySwarm;
    public void SetSwarm(Swarm swarm)
    {
        mySwarm = swarm;
    }

    private void FixedUpdate()
    {
        transform.position = mySwarm.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            mySwarm.SetPrey(collision.gameObject.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mySwarm.SetPrey(null);
        }
    }
}
