using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionOld : MonoBehaviour
{
    private List<GameObject> agentsList;    // Lista de agentes de flocking en el área de visión
    private Swarm flock;
    public GameObject flockPrefab;

    // Variables de información
    private bool isFlocking;    // Es miembro de un cardumen?

    private void Awake()
    {
        isFlocking = false;
        agentsList = new List<GameObject>();
    }

    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFlocking)
        {            
            if (collision.gameObject.tag.Equals("Flock"))
            {
                isFlocking = true;
                flock = collision.gameObject.GetComponent<Swarm>();
            }
            if (collision.gameObject.tag.Equals("Mosquito"))
            {
                Instantiate(flockPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isFlocking)
        {
            if (collision.gameObject.tag.Equals("Flock") && collision.gameObject.Equals(flock.gameObject))
            {
                isFlocking = false;
                flock = null;
            }
        }     
    }

    public List<GameObject> GetAllAgents()
    {
        return agentsList;
    }

    public Vector2 GetFlockPosition()
    {
        return flock.GetFlockPosition();
    }

    public bool IsFlocking()
    {
        return isFlocking;
    }

    public void ChangeFlock(Swarm otherFlock)
    {
        flock = otherFlock;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
