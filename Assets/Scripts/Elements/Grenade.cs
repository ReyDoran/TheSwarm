using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase granada. Explota al colisionar con X mosquitos simultáneamente y elimina a todos los que estén en el radio de explosión
/// Tiene dos estados: cohete y explosión.
/// El cohete se convierte en explosión al colisionar con X mosquitos
/// Cuando es explosión, elimina a los mosquitos que entren en su radio durante x segundos, y luego se destruye
/// No utiliza la pool ya que habrá poco uso
/// </summary>
public class Grenade : MonoBehaviour
{
    #region VARIABLES
    // Públicos
    public Vector2 movement;    // Velocidad

    // Referencias
    private ParticleSystem particleSystem;
    private AudioSource audioSource;

    // Ajustes
    private int collisionsToExplode = 4;
    private float explosionRadius = 5f;
    private float explosionDuration = 0.5f;

    // Datos
    private bool isExploding;
    private int collisions;
    private float timeElapsed;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        var main = particleSystem.main;
        main.stopAction = ParticleSystemStopAction.Callback;
        isExploding = false;
        collisions = 0;
        timeElapsed = 0;
    }

    /// <summary>
    /// En modo cohete (isExploding == false), al colisionar simultaneamente con tantos mosquitos como indique collisionsToExplode, explotará.
    /// En modo explosión (isExploding == true), eliminará los mosquitos que se encuentren dentro de su radio durante x segundos. 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isExploding)
        {
            collisions++;
            if (collisions >= collisionsToExplode)
            {
                isExploding = true;
                particleSystem.Play();
                audioSource.Play();                
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
                Collider2D[] mosquitosInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                foreach(Collider2D collider in mosquitosInRange)
                {
                    Mosquito mosquito = collider.GetComponent<Mosquito>();
                    if (mosquito != null)
                        mosquito.Kill();
                }
            }
        } else
        {
            collision.GetComponent<Mosquito>().Kill();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isExploding)
            collisions--;
    }

    /// <summary>
    /// En modo cohete se mueve en la dirección indicada
    /// En modo explosión comprueba si han pasado X segundos y se destruye
    /// </summary>
    private void FixedUpdate()
    {
        if (!isExploding)
        {
            transform.position = transform.position + new Vector3(movement.x * Time.deltaTime, movement.y * Time.deltaTime, 0);
        } else
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= explosionDuration)
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Asigna el movimiento de la granada
    /// </summary>
    /// <param name="movement"></param>
    public void SetMovement(Vector2 movement)
    {
        transform.forward = new Vector3(movement.x, movement.y, 0);
        this.movement = movement;
    }
    #endregion
}
