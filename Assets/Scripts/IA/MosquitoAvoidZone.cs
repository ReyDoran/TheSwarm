using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zona que repele a los mosquitos. Sirve para que esquiven proyectiles o hagan formas determinadas
/// Puede actuar también a la inversa, atrayendo a los mosquitos
/// Cuando un mosquito está dentro de su colisión, llama a Avoid con el vector que une su centroide al mosquito
/// Sigue al objetivo target
/// Se almacena en la pool de gameobjects
/// </summary>
public class MosquitoAvoidZone : MonoBehaviour
{
    #region VARIABLES
    // Referencias
    public MainPool mainPool;

    // Ajustes
    private float defaultRadius = 2f;
    private const int defaultMaxNumFollowers = 15;
    private const float defaultAttractRadius = 2f;

    // Datos
    private List<Mosquito> mosquitosList;
    private bool isAttractZone = false;
    private int maxFollowers;
    private int followers = 0;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        mosquitosList = new List<Mosquito>();
    }

    private void OnEnable()
    {
        mosquitosList.Clear();
        isAttractZone = false;
        followers = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttractZone && followers < maxFollowers && !collision.gameObject.GetComponent<Mosquito>().isLeader && !collision.gameObject.GetComponent<Mosquito>().isFollower)
        {
            collision.gameObject.GetComponent<Mosquito>().isFollower = true;
            collision.gameObject.GetComponent<Mosquito>().myLeaderZone = this;
            mosquitosList.Add(collision.gameObject.GetComponent<Mosquito>());
            followers++;
        } else if (!isAttractZone)
        {
            mosquitosList.Add(collision.gameObject.GetComponent<Mosquito>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (mosquitosList.Remove(collision.gameObject.GetComponent<Mosquito>()))
        {
            if (isAttractZone)
                followers--;
            collision.gameObject.GetComponent<Mosquito>().isFollower = false;
            collision.gameObject.GetComponent<Mosquito>().myLeaderZone = null;
        }
    }
    private void FixedUpdate()
    {
        foreach (Mosquito mosquito in mosquitosList)
        {
            Vector2 direction = new Vector2(mosquito.transform.position.x - transform.position.x, mosquito.transform.position.y - transform.position.y);
            if (!isAttractZone)
                mosquito.Avoid(direction);
            else
            {
                mosquito.Avoid(-direction);
            }
        }
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Se devuelve al pool
    /// </summary>
    public void ReturnToPool()
    {
        GetComponent<CircleCollider2D>().radius = defaultRadius;
        followers = 0;
        mainPool.AddAvoidZone(gameObject);
    }

    /// <summary>
    /// Asigna el target de la MosquitoAvoidZone
    /// Se sitúa en su posición y lo sigue
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        transform.position = target.position;
        transform.SetParent(target);
    }

    /// <summary>
    /// Settea si es zona de evitar o atraer
    /// </summary>
    /// <param name="isAttract"></param>
    public void SetAttract(bool isAttract, int maxNumFollowers = defaultMaxNumFollowers, float attractRadius = defaultAttractRadius)
    {
        isAttractZone = isAttract;
        maxFollowers = maxNumFollowers;
        GetComponent<CircleCollider2D>().radius = attractRadius;
    }

    public void InitAsLeaderZone(Transform target, int maxNumFollowers = defaultMaxNumFollowers, float attractRadius = defaultAttractRadius)
    {
        SetTarget(target);
        SetAttract(true, maxNumFollowers, attractRadius);
    }

    /// <summary>
    /// Libera el mosquito pasado como parámetro de la lista de seguidores
    /// </summary>
    /// <param name="mosquito"></param>
    public void FreeMosquito(Mosquito mosquito)
    {
        mosquitosList.Remove(mosquito);
    }
    #endregion
}
