using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase llama. Puede ser lanzada (por el lanzallamas) o unida a un mosquito (ardiendo).
/// Contagia a otros mosquitos que toca (tantos como el valor de maxCollisions)
/// Para ser lanzada usar InitAsProjectile() para asignarle la velocidad, y dura tanto tiempo como indique lifeSpan
/// Para ser unida a un mosquito usar InitAsAttached() para asignarla a un target. Debe ser eliminada (lifeSpan no aplica)
/// Tiene máximo de llamas activadas por unidad de tiempo para evitar bajadas de fps
/// </summary>
public class Flame : MonoBehaviour
{
    #region VARIABLES
    // Static
    // Controla el nº máximo de contagios de todas las instancias de llama por intervalo de tiempo
    private static int flameSpreads = 0;
    private static float timeElapsed = 0;

    // Referencias
    public MainPool mainPool; // Pool para devolverse al extinguirse

    // Datos
    public Vector2 movement;    // Velocidad (lanzada por el lanzallamas)
    private float actualLifeSpan;   // Auxiliar de tiempo que permanece viva (configurar lifeSpan)
    private int collisions; // Veces que ha colisionado (y contagiado a otro mosquito)
    private float timeAlive;    // Tiempo que lleva encendida (aplica solo llamas lanzadas)
    public bool isProjectile;   // Es de tipo proyectil?

    // Ajustes
    private const float timeToRefreshSpreads = 0.2f;    // Cada cuanto se permiten más contagios
    private const int maxSpreadsPerSegment = 150;    // Cuantos contagios se permiten por unidad de tiempo
    private int maxCollisions = 1;  // Cuantas veces puede contagiar cada instancia
    private float lifeSpan = 2.5f;  // Tiempo que aguanta viva (aplica sólo en proyectil)
    #endregion VARIABLES

    #region UNITY CALLBACKS
    private void Awake()
    {
        GetComponent<CircleCollider2D>().enabled = true;
        collisions = 0;
        timeAlive = 0f;
        actualLifeSpan = lifeSpan;
        isProjectile = false;
        transform.parent = null;
    }

    private void OnEnable()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<CircleCollider2D>().enabled = true;
        collisions = 0;
        timeAlive = 0f;
        actualLifeSpan = lifeSpan;
        isProjectile = false;
        transform.SetParent(null);
    }

    private void OnDisable()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        collisions = 0;
        timeAlive = 0f;
        actualLifeSpan = lifeSpan;
        isProjectile = false;
        transform.SetParent(null);
    }

    /// <summary>
    /// Cada vez que toca a un mosquito lo quema (si no está ya ardiendo y si quedan activaciones disponibles)
    /// Si llega a su máximo de quemados, desactiva su collider
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsSpreadAvaliable())
        {
            if (collision.gameObject.GetComponent<Mosquito>().Burn())
            {
                SpreadFlame();
            }
            collisions++;
            if (collisions >= maxCollisions)
            {
                if(isProjectile)
                    Extinguish();
                GetComponent<CircleCollider2D>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Si es proyectil, se mueve con los valores de movement y se elimina al pasar el tiempo marcado por lifeSpan
    /// </summary>
    private void FixedUpdate()
    {
        if(isProjectile)
        {
            gameObject.transform.position = gameObject.transform.position + new Vector3(Time.deltaTime * movement.x, Time.deltaTime * movement.y, 0f);
            timeAlive += Time.deltaTime;
            if (timeAlive > actualLifeSpan)
            {
                Extinguish();
            }
        }        
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Convierte la llama en un proyectil con la velocidad pasada como parámetro
    /// Se apaga al pasar el tiempo predeterminado
    /// </summary>
    /// <param name="speed"></param>
    public void InitAsProjectile(Vector2 speed)
    {
        isProjectile = true;
        movement = speed;
        GetComponentInChildren<ParticleSystem>().Play();
    }

    /// <summary>
    /// Convierte la llama en un proyectil con la velociday el tiempo de vida pasados como parámetro
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="timeToDie"></param>
    public void InitAsProjectile(Vector2 speed, float timeToDie)
    {
        actualLifeSpan = timeToDie;
        isProjectile = true;
        movement = speed;
        GetComponentInChildren<ParticleSystem>().Play();
    }

    /// <summary>
    /// Convierte la llama en una llama asociada a un gameObject.
    /// </summary>
    /// <param name="targetTransform"></param>
    public void InitAsAttached(Transform targetTransform)
    {
        isProjectile = false;
        transform.parent = targetTransform;
        GetComponentInChildren<ParticleSystem>().Play();
    }

    /// <summary>
    /// Extingue la llama
    /// Se devuelve al pool y resetea sus datos
    /// </summary>
    public void Extinguish()
    {
        mainPool.AddFlame(gameObject);       
    }
    #endregion

    #region STATIC METHODS
    /// <summary>
    /// Contabiliza un contagio de llamas
    /// Lleva la cuenta de todas las nuevas llamas creadas en cada intervalo de tiempo
    /// Debe llamarse sólo si se ha obtenido true en IsSpreadAvaliable()
    /// Si ha pasado el tiempo suficiente, resetea los contagios disponibles
    /// </summary>
    /// <returns></returns>
    public static void SpreadFlame()
    {
        if (Time.time - timeElapsed > timeToRefreshSpreads)
        {
            timeElapsed = Time.time;
            flameSpreads = 0;
        }
        flameSpreads++;
    }

    /// <summary>
    /// Devuelve si hay contagios de llama disponbiles en el intervalo de tiempo
    /// </summary>
    /// <returns></returns>
    public static bool IsSpreadAvaliable()
    {
        return flameSpreads < maxSpreadsPerSegment;
    }
    #endregion
}
