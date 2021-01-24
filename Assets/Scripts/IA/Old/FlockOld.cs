using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockOld : MonoBehaviour
{
    private List<GameObject> agentsList;
    private Vector2 flockPosition;
    private int iteration;

    // Información
    private bool isDestroyed;

    private void Awake()
    {
        agentsList = new List<GameObject>();
        flockPosition = new Vector2(0f, 0f);
        isDestroyed = false;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (iteration == 10)
        {
            iteration = 0;
            flockPosition = CalculateFlockPosition();
        }
        iteration++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.gameObject.tag.Equals("Mosquito"))
        {
            agentsList.Add(collision.attachedRigidbody.gameObject);
        } else if (collision.gameObject.tag.Equals("Flock"))
        {
            if (!collision.gameObject.GetComponent<FlockOld>().isDestroyed)
            {
                ChangeOfFlock(collision.gameObject.GetComponent<Swarm>());
                isDestroyed = true;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Mosquito"))
        {
            agentsList.Remove(collision.attachedRigidbody.gameObject);
            if (agentsList.Count <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ChangeOfFlock(Swarm otherFlock)
    {
        foreach(GameObject agent in agentsList)
        {
            agent.GetComponentInChildren<PerceptionOld>().ChangeFlock(otherFlock);
        }
    }

    public Vector2 GetFlockPosition()
    {
        return flockPosition;
    }

    private Vector2 CalculateFlockPosition()
    {
        Vector2 averagePosition = new Vector2(0f, 0f);
        foreach (GameObject agent in agentsList)
        {
            averagePosition += new Vector2(agent.transform.position.x, agent.transform.position.y);
        }
        averagePosition /= agentsList.Count;
        return averagePosition;
    }
}
