﻿using System.Collections;
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
    private Transform targetObject;
    private List<Mosquito> mosquitosList;
    private bool isAttractZone = false;
    private int maxFollowers;
    private int followers = 0;
    private bool isStrong;  // Es de tipo fuerte?
    private float extraForce; // Fuerza extra a aplicar
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        mosquitosList = new List<Mosquito>();
        isStrong = false;
    }

    private void OnEnable()
    {
        mosquitosList.Clear();
        isAttractZone = false;
        isStrong = false;
        followers = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttractZone)
        {
            if (isAttractZone && followers < maxFollowers && !collision.gameObject.GetComponent<Mosquito>().isLeader && !collision.gameObject.GetComponent<Mosquito>().isFollower)
            {
                collision.gameObject.GetComponent<Mosquito>().isFollower = true;
                collision.gameObject.GetComponent<Mosquito>().myLeaderZone = this;
                mosquitosList.Add(collision.gameObject.GetComponent<Mosquito>());
                followers++;
            }
            else if (!isAttractZone && !collision.gameObject.Equals(targetObject.gameObject))
            {
                mosquitosList.Add(collision.gameObject.GetComponent<Mosquito>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAttractZone)
        {
            if (mosquitosList.Remove(collision.gameObject.GetComponent<Mosquito>()))
            {
                if (isAttractZone)
                {
                    followers--;
                    collision.gameObject.GetComponent<Mosquito>().isFollower = false;
                    collision.gameObject.GetComponent<Mosquito>().myLeaderZone = null;
                }
            }
        }        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 direction = new Vector2(collision.gameObject.transform.position.x - transform.position.x, collision.gameObject.transform.position.y - transform.position.y);
        collision.gameObject.GetComponent<Mosquito>().Avoid(direction, extraForce);
    }

    private void FixedUpdate()
    {
        if (isAttractZone)
        {
            foreach (Mosquito mosquito in mosquitosList)
            {
                Vector2 direction = new Vector2(mosquito.transform.position.x - transform.position.x, mosquito.transform.position.y - transform.position.y);
            //if (!isAttractZone)
                //mosquito.Avoid(direction, extraForce);
            //else

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
        transform.SetParent(null);
        mainPool.AddAvoidZone(gameObject);
    }

    /// <summary>
    /// Asigna el target de la MosquitoAvoidZone
    /// Se sitúa en su posición y lo sigue
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target, float forceApplied = 1f)
    {
        targetObject = target;
        transform.position = target.position;
        transform.SetParent(target);
        extraForce = forceApplied;
    }

    /// <summary>
    /// Se destruye (vuelve a la pool) en X segundos
    /// </summary>
    /// <param name="timeToReturn"></param>
    public void ReturnToPoolDelayed(float timeToReturn)
    {
        Invoke(nameof(ReturnToPool), timeToReturn);
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
