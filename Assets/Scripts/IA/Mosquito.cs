using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Agente de flocking que representa un mosquito standard
/// Tiene movimiento aleatorio y contiene diversos métodos para interactuar con él (Burn(), Kill(), SetLeader()...)
/// Si es miembro de un enjambre sigue su posición con comportamiento parecido al de un enjambre de insectos
/// El enjambre es quien piensa y le ordena cambios de formación u otros
/// No comprueba ningún tipo de colisiones (los elementos que interactúen con él deben ordenarle)
/// De esta clase heredan los mosquitos especiales
/// </summary>
public class Mosquito : MonoBehaviour
{
    #region VARIABLES
    // 1) Referencias
    private Transform transform;
    protected Rigidbody2D rigidbody;
    protected Transform meshTransform;
    private CircleCollider2D circleCollider;
    public MainPool mainPool; // Referencia al pool de mosquitos para los mosquitos eliminados

    // 2) Datos
    public Swarm mySwarm;   // Referencia al flock al que pertenece
    public float mySwarmRadius; // Radio del flock
    public Formations formation;    // Formación del flock
    protected Vector2 myPosition;   
    protected Vector2 randomForce;    // Velocidad aleatoria aplicada (cambia sólo en algunos frames)
    protected Vector2 avoidForce;   // Fuerza de esquivar aplicada
    public bool isLeader = false;   // Es líder de escuadrón?
    public bool isFollower = false; // Es segudor de escuadrón?
    public bool isDead = false; // Está muerto?
    public bool isBurning = false;  // Está ardiendo el mosquito?
    public bool isFlocking = false; // Está dentro de un enjambre?
    public bool avoidRequested = false; // Está dentro de una avoidZone?
    protected GameObject myFlame;
    protected GameObject myAvoidZone; // Zona de evitar de mosquitos asociado a la llama
    public MosquitoAvoidZone myLeaderZone;  // Zona de atracción de líder de escuadrón (ya sea el líder o seguidor)
    public int damage = 1;  // Daño al colisionar

    // 3) Ajustes
    // 3.1) Multiplicadores de fuerza
    protected float cohesionForceModifier = 11f;   // Multiplicador de fuerza de cohesión
    private float proximityCohesionModifier; // Multiplicador de la reducción de la fuerza de cohesión en función de la distancia al centro (-0.9 fuerza máxima en el borde, 1 siempre máxima fuerza)
    private float proximityCohesionModifierStandard = 0.5f; // Valor del multiplicador anterior en caso de formación standard
    private float proximityCohesionModifierCircle = 0.1f; // Valor del multiplicador anterior en caso de formación círculo
    private float proximityCohesionModifierDisperse = -0.3f;    // Valor del multiplicador anterior en caso de formación dispersa
    private float circleCohesionForceModifier = 10f; // Multiplicador del aumento de fuerza de cohesión para la formación círculo
    private float radiusPercentage = 0.2f;   // Porcentaje del radio del flock que ocuparán los mosquitos (0.2f)
    private float avoidForceModifier = 30f;     // Multiplicador de fuerza de evasión
    private float randomForceModifier = 7f;    // Multiplicador de fuerza aleatoria
    private float randomForceZModifier = 2f; // Mulitplicador de velocidad aleatoria en el eje Z
    // 3.2) Caps
    protected float maxSpeed = 5f;    // Velocidad máxima (5)
    protected float maxForceAdded = 2f;  // Fuerza máxima añadia por frame (15)
    private float minPositionZ = -2.2f;    // Posición máxima del mosquito en el eje z
    private float maxPositionZ = -4.3f;   // Posición mínima del mosquito en el eje z
    // 3.3) Otros
    private float randomDirectionChangeModifier = 0.08f;    // Frecuencia de cambio de dirección aleatoria (0 - nunca, 1 - cada frame)
    private float burningTime; // Tiempo que tarda en morir al arder

    // 4) Auxiliares
    private float changeRandomDirection = 0f;
    private bool flyingUpwards = false;
    #endregion VARIABLES

    #region UNITY CALLBACKS
    protected void Awake()
    {
        burningTime = Random.Range(1.8f, 3.0f);
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody2D>();
        meshTransform = GetComponentInChildren<MeshRenderer>().transform;
        circleCollider = GetComponent<CircleCollider2D>();
        myPosition = new Vector2(0f, 0f);
        SetFormation(Formations.Standard);
    }

    protected void OnEnable()
    {
        transform.SetParent(null);
    }

    private void OnDisable()
    {
        if (isFlocking)
        {
            mySwarm.RemoveAgent(gameObject);
        }
    }

    /// <summary>
    /// Llama a las funciones de movimiento
    /// </summary>
    private void FixedUpdate()
    {
        ApplyFlockingForces();
        ApplyRandomZ(Time.fixedDeltaTime);
    }
    #endregion UNITY CALLBACKS

    #region PUBLIC METHODS
    /// <summary>
    /// Asigna el flock del mosquito
    /// </summary>
    /// <param name="myFlock"></param>
    public void SetFlock(Swarm myFlock)
    {
        this.mySwarm = myFlock;
        isFlocking = true;
        if (isBurning)
            mySwarm.isInFlames++;
    }

    /// <summary>
    /// Elimina el enjambre del mosquito
    /// </summary>
    public void RemoveFlock()
    {
        if (isBurning && mySwarm != null)
            mySwarm.isInFlames--;
        this.mySwarm = null;
        transform.SetParent(null);
        isFlocking = false;
    }

    /// <summary>
    /// Asigna la formación al mosquito (y cambia las variables necesarias)
    /// </summary>
    /// <param name="newFormation"></param>
    public void SetFormation(Formations newFormation)
    {
        formation = newFormation;
        switch (formation)
        {
            case Formations.Standard:
                proximityCohesionModifier = proximityCohesionModifierStandard;
                break;
            case Formations.Circle:
                proximityCohesionModifier = proximityCohesionModifierCircle;
                break;
            case Formations.Disperse:
                proximityCohesionModifier = proximityCohesionModifierDisperse;
                break;
        }
    }

    /// <summary>
    /// Mata el mosquito, lo elimina del flock y lo desactiva
    /// </summary>
    public virtual void Kill()
    {
        if (isFlocking)
        {
            mySwarm.RemoveAgent(gameObject);
            isFlocking = false;
        }
        if (isBurning)
        {
            if (mySwarm != null)
                mySwarm.isInFlames--;
            myFlame.GetComponent<Flame>().Extinguish();
            myAvoidZone.GetComponent<MosquitoAvoidZone>().ReturnToPool();
        }
        mainPool.AddMosquito(gameObject);
        isDead = true;
        mainPool.RequestBlood(meshTransform.position);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Envuelve en llamas al mosquito
    /// Devuelve true si lo envuelve en llamas, false si no ha sido posible
    /// </summary>
    public virtual bool Burn()
    {
        bool wasSetOnFire = false;
        if (!isBurning)
        {
            wasSetOnFire = true;
            isBurning = true;
            mySwarm.isInFlames++;
            Transform auxParent = transform.parent;
            circleCollider.enabled = false;
            transform.parent = auxParent;
            myFlame = mainPool.GetFlame();
            myFlame.GetComponent<Flame>().InitAsAttached(transform.GetChild(0));
            myFlame.transform.position = transform.GetChild(0).position;
            myAvoidZone = mainPool.GetAvoidZone();
            myAvoidZone.GetComponent<MosquitoAvoidZone>().SetTarget(transform);
            Invoke("Kill", burningTime);
        }
        return wasSetOnFire;
    }

    /// <summary>
    /// Aplica una fuerza al mosquito en la dirección indicada
    /// Sirve para que las avoidZones le indiquen hacia dónde escapar
    /// </summary>
    /// <param name="direction"></param>
    public void Avoid(Vector2 direction, float extraForce = 1.0f)
    {
        avoidRequested = true;
        avoidForce = direction * avoidForceModifier * extraForce;
    }

    /// <summary>
    /// Convierte o le quita al mosquito la propiedad de ser líder
    /// Devuelve true si convierte a líder (si no lo era antes)
    /// Devuelve false si ya era líder (y se intenta convertir)
    /// Devuelve true en cualquier caso si se pide quitar la propiedad de líder
    /// Si es líder, atrae a los mosquitos alrededor suya dentro de un radio
    /// </summary>
    /// <param name="leader"></param>
    /// <returns></returns>
    public bool SetLeader(bool leader, int followers = 10)
    {
        if (leader)
        {
            if (isLeader || isBurning)
                return false;
            isLeader = true;
            if (myAvoidZone == null)
                myAvoidZone = mainPool.GetAvoidZone();
            myAvoidZone.GetComponent<MosquitoAvoidZone>().InitAsLeaderZone(transform, followers);
            if (myLeaderZone != null)
                myLeaderZone.FreeMosquito(this);
            return true;
        } else {
            isLeader = false;
            if (!isBurning)
            {
                if (myAvoidZone != null)
                {
                    myAvoidZone.GetComponent<MosquitoAvoidZone>().ReturnToPool();
                    myAvoidZone = null;
                }
            }
            return true;
        }
    }
    #endregion PUBLIC METHODS

    #region PRIVATE/PROTECTED METHODS
    /// <summary>
    /// Calcula la fuerza de cohesión en función de la formación actual del enjambre
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2 CalculateCohesionForce()
    {
        Vector2 cohesionForce = new Vector2(0f, 0f);
        float proximity;
        if (!isFlocking)
        {
            return cohesionForce;
        }
        switch (formation)
        {
            case Formations.Standard:
            case Formations.Disperse:
                cohesionForce = mySwarm.GetFlockPosition() - myPosition;    // Calculamos vector distancia
                proximity = Mathf.Clamp(cohesionForce.magnitude / mySwarmRadius + proximityCohesionModifier, 0f, 1f);   // Aplica un porcentaje dependiendo de lo lejos que esté del centro del cardumen       
                cohesionForce.Normalize();
                cohesionForce *= cohesionForceModifier * proximity; // Aplica multiplicador de fuerza
                break;

            case Formations.Circle:
                cohesionForce = mySwarm.GetFlockPosition() - myPosition;    // Calculamos vector distancia
                proximity = Mathf.Clamp(cohesionForce.magnitude / mySwarmRadius + proximityCohesionModifier, 0f, 1f);   // Aplica un porcentaje dependiendo de lo lejos que esté del centro del cardumen
                proximity -= (1 - radiusPercentage);
                Mathf.Clamp(proximity, -radiusPercentage, radiusPercentage);
                cohesionForce.Normalize();
                cohesionForce *= cohesionForceModifier * proximity * circleCohesionForceModifier; // Aplica multiplicador de fuerza
                break;
        }
        return cohesionForce;
    }

    /// <summary>
    /// Calcula una fuerza aleatoria
    /// Esta fuerza cámbia cada X tiempo con una frecuencia determinada por randomDirectionChangeModifier
    /// </summary>
    /// <returns></returns>
    protected Vector2 CalculateRandomForce()
    {
        changeRandomDirection += Random.Range(0, randomDirectionChangeModifier);
        if (changeRandomDirection > 1f)
        {
            randomForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            randomForce *= randomForceModifier;
            changeRandomDirection = 0f;
        }        
        return randomForce;
    }

    /// <summary>
    /// Obtiene y aplica las fuerzas de flocking así como los limitadores
    /// </summary>
    protected virtual void ApplyFlockingForces()
    {
        myPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 flockingForce = new Vector2(0f, 0f);

        // Obtiene las fuerzas        
        flockingForce += CalculateCohesionForce();
        flockingForce += CalculateRandomForce();
        if (avoidRequested == true)
        {
            avoidRequested = false;
            flockingForce += avoidForce;
        }
        //flockingForce += CalculateSeparationForce();
        Vector2.ClampMagnitude(flockingForce, maxForceAdded);

        // Aplica las fuerzas y el límite
        rigidbody.AddForce(flockingForce);
        ApplyCaps();
    }

    /// <summary>
    /// Aplica el límite de velocidad de movimiento
    /// </summary>
    protected void ApplyCaps()
    {
        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, maxSpeed);
    }

    /// <summary>
    /// Cambia la posición de la malla en el eje Z de forma aleatoria (sólo cambios visuales)
    /// No se tienen en cuenta fuerzas de flocking
    /// </summary>
    /// <param name="deltaTime"></param>
    private void ApplyRandomZ(float deltaTime)
    {
        float forceZ;
        // Si se sale de los límites aplicar fuerza en dirección opuesta
        if (meshTransform.position.z > minPositionZ)
        {
            forceZ = -randomForceZModifier;
            flyingUpwards = true;
        }
        else if (meshTransform.position.z < maxPositionZ)
        {
            forceZ = randomForceZModifier;
            flyingUpwards = false;
        }
        else // Si no se sale de los límites aplicar fuerza aleatoria con frecuencia dependiente de segundo valor del if (pasar a variable de clase)
        {
            if (Random.Range(0f, 1f) > 0.98f)
            {
                flyingUpwards = !flyingUpwards;
                if (!isFlocking && transform.parent != null)
                    transform.SetParent(null);
            }
            if (flyingUpwards == true)
            {
                forceZ = -randomForceZModifier;
            }
            else
            {
                forceZ = randomForceZModifier;
            }
        }
        // Aplica la velocidad cambiando la posición directamente en función del tiempo pasado
        meshTransform.position = new Vector3(meshTransform.position.x, meshTransform.position.y, meshTransform.position.z + forceZ * deltaTime);
    }
    #endregion
}
