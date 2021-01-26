using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de enjambres, agente inteligente que ordena a los mosquitos
/// Anexiona los mosquitos que entran en su área de influencia y libera los que salen y mueren
/// Les comunica determinados parámetros, la formación y les permite obtener posición media del enjambre
/// No tiene ningún mosquito al instanciarse
/// 
/// Controlado por una máquina de estados jerárquica
/// Los estados pueden utilizar los métodos públicos:
/// - SetFormation(Formations newFormation)
/// - SetFlamesDefense(bool def)
/// - Controlar directamente la posición y rotación a través de swarmMovement
/// Los estados reciben información de:
/// - Cambio visibilidad presa, cambio arma presa, nº mosquitos del enjambre
/// </summary>
public class Swarm : MonoBehaviour
{
    #region VARIABLES
    // Referencias
    private CircleCollider2D circleCollider;
    private CircleCollider2D perceptionCircleCollider;   // Collider de la percepción
    private Transform transform;

    // Datos
    private List<GameObject> mosquitosList;    // Lista de agentes dentro del enjambre (de todos los tipos)
    private List<GameObject> leadersList;   // Lista de líderes
    private List<GameObject> phoenixMosquitosList;  // Lista de mosquitos fénix
    private List<GameObject> bulletMosquitosList;   // Lista de mosquitos bala
    private Vector2 averagePosition;    // Posición media del cardumen
    private Formations formation;   // Formación del enjambre
    private Transform prey; // Presa del enjambre (jugador)
    private IState myState; // Estado actual

    private bool isReady;   // Está lista? (tiene el nº mínimo de agentes para considerarse flock)
    public bool isAnnexable;    // Se puede anexionar?
    public bool isAnnexionating;     // Está anexionando?
    public bool isPreyInSight;  // Tiene presa a la vista?
    public int isInFlames; // Está el flock en llamas?

    // Ajustes
    private float swarmRadius = 10f;    // Radio del flock (no es el mismo el radio del flock que el radio para captar nuevos mosquitos para el flock)
    private float baseSwarmSize = 7.5f;  // Tamaño mínimo del flock
    private float swarmSizeDivisor = 10f;   // Factor de aumento de tamaño del flock por cada mosquito
    private float swarmForcesRadiusMultiplier; // Factor de disminución del swarm para los mosquitos
    private float swarmForcesRadiusMultiplierStandard = 0.8f;    // Valor anterior variable para formación standard
    private float swarmForcesRadiusMultiplierCircle = 1f;    // Valor anterior variable para formación círculo
    private float swarmForcesRadiusMultiplierDisperse = 0.9f;    // Valor anterior variable para formación dispersa
    private float perceptionRadiusMultiplier = 3.5f;  // Multiplicador del radio de la percepción respecto al del enjambre
    private int minAgentsPerFlock = 5; // Mínimo de agentes para ser considerado flock
    private int annexableTreshold = 20;    // Máximo de agentes hasta convertirse en no anexionable
    private float timesBiggerToAnnex = 3f;    // nº de veces más grande (en nº de unidades) para que un enjambre anexione a otro

    // Percepción
    public GameObject perceptionPrefab;
    public GameObject myPerception;

    // Controles para los estados
    public GameObject swarmMovementPrefab;
    public GameObject swarmMovement;    // A través de swarmMovement puede mover la posición y rotación del flock directamente
    
    #endregion VARIABLES

    #region UNITY CALLBACKS
    private void Awake()
    {
        transform = GetComponent<Transform>();
        circleCollider = GetComponent<CircleCollider2D>();
        swarmRadius = baseSwarmSize;
        circleCollider.radius = swarmRadius;
        mosquitosList = new List<GameObject>();
        leadersList = new List<GameObject>();
        phoenixMosquitosList = new List<GameObject>();
        bulletMosquitosList = new List<GameObject>();
        SetFormation(Formations.Standard);
        isReady = false;
        isAnnexable = false;
        isPreyInSight = false;
        isInFlames = 0;
        swarmMovement = Instantiate(swarmMovementPrefab, transform.position, Quaternion.identity);
        myState = new WanderingState(this);
        myPerception = Instantiate(perceptionPrefab, transform.position, Quaternion.identity);
        myPerception.GetComponent<SwarmPerception>().SetSwarm(this);
        perceptionCircleCollider = myPerception.GetComponent<CircleCollider2D>();
    }

    /// <summary>
    /// Calcula la posición media del enjambre y se sitúa en ella
    /// </summary>
    private void FixedUpdate()
    {
        CalculateAveragePosition();
        transform.position = averagePosition;
        myState.ExecuteState();
    }

    /// <summary>
    /// Si se encuentra con un mosquito libre lo anexiona
    /// Si se encuentra con otro enjambre anexionable más pequeño lo anexiona
    /// Si es anexionable y se encuentra con otro enjambre más grande se une a él
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Mosquito"))
        {
            if (!collision.gameObject.GetComponent<Mosquito>().isFlocking)
                AddAgent(collision.gameObject);
        }
        else if (collision.gameObject.tag.Equals("Flock"))
        {
            if (this.GetInstanceID() > collision.gameObject.GetInstanceID())
            {
                Swarm otherFlock = collision.gameObject.GetComponent<Swarm>();
                if (GetAgentsCount() > otherFlock.GetAgentsCount() * timesBiggerToAnnex && otherFlock.isAnnexable)
                {
                    List<GameObject> otherFlockAgents = otherFlock.GetAllAgents();
                    while (otherFlockAgents.Count > 0)
                    {
                        AddAgent(otherFlock.RemoveAgent(otherFlockAgents[0], false));
                    }
                }
                else if (GetAgentsCount() < otherFlock.GetAgentsCount() * timesBiggerToAnnex && isAnnexable)
                {
                    while (mosquitosList.Count > 0)
                    {
                        otherFlock.AddAgent(RemoveAgent(mosquitosList[0], false));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Si un mosquito sale del radio de acción, lo elimina del enjambre
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Mosquito"))
        {
            RemoveAgent(collision.gameObject);
        }
    }
    #endregion UNITY CALLBACKS

    #region PRIVATE METHODS
    /// <summary>
    /// Inicializa el flock
    /// </summary>
    private void InitializeFlock()
    {
        isReady = true;
        swarmMovement.transform.position = averagePosition;
        transform.SetParent(swarmMovement.transform);
        foreach (GameObject mosquito in mosquitosList)
        {
            mosquito.transform.SetParent(swarmMovement.transform);
        }
        myState.InitState();
    }

    /// <summary>
    /// Inicializa al mosquito con los valores del enjambre
    /// </summary>
    /// <param name="mosquito"></param>
    private void SetMosquitoValues(Mosquito mosquito)
    {
        mosquito.SetFlock(this);
        mosquito.SetFormation(formation);
        mosquito.mySwarmRadius = swarmRadius;
    }

    /// <summary>
    /// Calcula la posición media del enjambre
    /// </summary>
    private void CalculateAveragePosition()
    {
        averagePosition = Vector2.zero;
        if (mosquitosList.Count > 0)
        {
            foreach (GameObject agent in mosquitosList)
            {
                if (!agent.GetComponent<Mosquito>().isDead)
                {
                    averagePosition += new Vector2(agent.transform.position.x, agent.transform.position.y);
                }
            }
            averagePosition = averagePosition / mosquitosList.Count;
        } else
        {
            averagePosition = transform.position;
        }
    }

    /// <summary>
    /// Método wrapper para comunicar información a los estados
    /// En caso de cambio de estado, termina el actual e inicia el siguiente
    /// </summary>
    /// <param name="state"></param>
    private void CheckStateChange(IState state)
    {
        if (!state.Equals(myState))
        {
            myState.EndState();
            state.InitState();
            myState = state;
        }
    }


    /// <summary>
    /// Actualiza el radio del flock y del área de influencia en función del nº de agentes que contiene
    /// </summary>
    /// <param name="gameObject"></param>
    private void ActualizeFlockRadius(bool destroyAuthorization = false)
    {
        if (destroyAuthorization && mosquitosList.Count < minAgentsPerFlock || mosquitosList.Count == 0)
        {
            DestroyFlock();
        } else
        {
            swarmRadius = baseSwarmSize + mosquitosList.Count / swarmSizeDivisor;
            perceptionCircleCollider.radius = swarmRadius * perceptionRadiusMultiplier;
            circleCollider.radius = swarmRadius;
            foreach (GameObject agent in mosquitosList)
            {
                agent.GetComponent<Mosquito>().mySwarmRadius = swarmRadius * swarmForcesRadiusMultiplier;
            }
        }
    }

    /// <summary>
    /// Destruye el flock
    /// </summary>
    private void DestroyFlock()
    {
        if (mosquitosList != null && mosquitosList.Count > 0)
        {
            while (mosquitosList.Count > 0)
            {
                GameObject agent = mosquitosList[0];
                agent.GetComponent<Mosquito>().RemoveFlock();
                mosquitosList.Remove(agent);
            }
        }
        Destroy(perceptionCircleCollider.gameObject);
        Destroy(swarmMovement);
        Destroy(gameObject);
    }

    /// <summary>
    /// Devuelve la lista con todos los agentes del flock
    /// </summary>
    /// <returns></returns>
    private List<GameObject> GetAllAgents()
    {
        return mosquitosList;
    }
    #endregion PRIVATE METHODS

    #region PUBLIC METHODS
    /// <summary>
    /// Cambia la formación y lo comunica a sus mosquitos
    /// </summary>
    public void SetFormation(Formations newFormation)
    {
        if (!formation.Equals(newFormation))
        {
            formation = newFormation;
            switch (formation)
            {
                case Formations.Standard:
                    foreach (GameObject agent in mosquitosList)
                    {
                        Mosquito mosquito = agent.GetComponent<Mosquito>();
                        swarmForcesRadiusMultiplier = swarmForcesRadiusMultiplierStandard;
                        ActualizeFlockRadius();
                        mosquito.SetFormation(formation);
                    }
                    break;

                case Formations.Circle:
                    foreach (GameObject agent in mosquitosList)
                    {
                        Mosquito mosquito = agent.GetComponent<Mosquito>();
                        swarmForcesRadiusMultiplier = swarmForcesRadiusMultiplierCircle;
                        ActualizeFlockRadius();
                        mosquito.SetFormation(formation);
                    }
                    break;

                case Formations.Disperse:
                    foreach (GameObject agent in mosquitosList)
                    {
                        Mosquito mosquito = agent.GetComponent<Mosquito>();
                        swarmForcesRadiusMultiplier = swarmForcesRadiusMultiplierDisperse;
                        ActualizeFlockRadius();
                        mosquito.SetFormation(formation);
                    }
                    break;
            }
        }        
    }

    /// <summary>
    /// Activa/desctiva la defensa de los mosquitos fénix
    /// </summary>
    /// <param name="defense"></param>
    public void SetFlamesDefense(bool defense)
    {
        foreach (GameObject pMosquito in phoenixMosquitosList)
        {
            pMosquito.GetComponent<PhoenixMosquito>().SetFrontline(defense);
        }
    }

    /// <summary>
    /// Dispara un mosquito bala en caso de disponer de ellos
    /// </summary>
    /// <param name="numberOfMosquitos"></param>
    public void FireBulletMosquito(int numberOfMosquitos = 1)
    {
        while(numberOfMosquitos > 0)
        {
            if (bulletMosquitosList.Count > 0)
            {
                bulletMosquitosList[Random.Range(0, bulletMosquitosList.Count)].GetComponent<BulletMosquito>().Attack();
            }
            numberOfMosquitos--;
        }
    }


    /// <summary>
    /// Añade el mosquito nuevo a su lista correspondiente y le settea los parámetros
    /// Recalcula el radio del enjambre
    /// </summary>
    /// <param name="newMosquito"></param>
    public void AddAgent(GameObject newMosquito)
    {        
        SetMosquitoValues(newMosquito.GetComponent<Mosquito>());
        if (newMosquito.TryGetComponent<PhoenixMosquito>(out PhoenixMosquito newPhoenixMosquito))
        {
            phoenixMosquitosList.Add(newMosquito);
        } else if (newMosquito.TryGetComponent<BulletMosquito>(out BulletMosquito newBulletMosquito))
        {
            bulletMosquitosList.Add(newMosquito);
        }
        mosquitosList.Add(newMosquito);
        if (mosquitosList.Count >= annexableTreshold)
            isAnnexable = false;
        ActualizeFlockRadius();
        if (isReady)
        {
            newMosquito.transform.SetParent(swarmMovement.transform);
        }
        else
        {
            if (mosquitosList.Count == minAgentsPerFlock)
            {
                CalculateAveragePosition();
                transform.position = averagePosition;
                InitializeFlock();
            }
        }
    }

    /// <summary>
    /// Elimina agente del cardumen, siempre que sea el dueño
    /// El segundo parámetro indica si se puede deshacer el flock (en caso de que esté inicializado)
    /// </summary>
    /// <param name="removedMosquito"></param>
    /// <returns></returns>
    public GameObject RemoveAgent(GameObject removedMosquito, bool destroyAuthorization = true)
    {
        if (!this.Equals(removedMosquito.GetComponent<Mosquito>().mySwarm))
            return removedMosquito;
        removedMosquito.GetComponent<Mosquito>().RemoveFlock();
        if (removedMosquito.TryGetComponent<PhoenixMosquito>(out PhoenixMosquito newPhoenixMosquito))
        {
            phoenixMosquitosList.Remove(newPhoenixMosquito.gameObject);
        } else if (removedMosquito.TryGetComponent<BulletMosquito>(out BulletMosquito newBulletMosquito))
        {
            bulletMosquitosList.Remove(newBulletMosquito.gameObject);
        }
        mosquitosList.Remove(removedMosquito);
        removedMosquito.transform.SetParent(null);
        if (mosquitosList.Count < annexableTreshold)
            isAnnexable = true;
        if (isReady && destroyAuthorization)
            ActualizeFlockRadius(true);
        else
            ActualizeFlockRadius();
        return removedMosquito;
    }

    /// <summary>
    /// Asigna la presa y el listener del cambio de arma
    /// Comunica al estado los cambios
    /// </summary>
    /// <param name="preyTransform"></param>
    public void SetPrey(Transform preyTransform)
    {
        if (preyTransform != null)
        {            
            prey = preyTransform;
            prey.gameObject.GetComponent<PlayerMovementController>().weaponSwapEvent.AddListener(PreyWeaponSwap);
            CheckStateChange(myState.ProcessData(true));
            isPreyInSight = true;
        } else
        {
            CheckStateChange(myState.ProcessData(false));
            isPreyInSight = false;
        }
    }

    /// <summary>
    /// Comunica al estado un cambio del arma de la presa
    /// </summary>
    public void PreyWeaponSwap()
    {
        myState.ProcessData(prey.GetComponent<PlayerMovementController>().drawnWeapon);
    }

    public Vector2 GetFlockPosition()
    {
        return averagePosition;
    }

    public Transform GetPrey()
    {
        if (prey != null)
            return prey;
        else
            return null;
    }

    public Vector2 GetPreyPosition()
    {
        if (prey != null)
            return prey.position;
        else
            return Vector2.zero;
    }

    public int GetMosquitosCount()
    {
        return mosquitosList.Count;
    }

    public int GetPhoenixMosquitosCount()
    {
        return phoenixMosquitosList.Count;
    }

    public int GetBulletMosquitosCount()
    {
        return bulletMosquitosList.Count;
    }

    public float GetRadius()
    {
        return swarmRadius;
    }

    public int GetAgentsCount()
    {
        return mosquitosList.Count;
    }
    #endregion PUBLIC METHODS

    #region TEST METHODS
    public void TestSquadronsAgrupation()
    {
        int numberOfLeaders = 29;
        while (numberOfLeaders > 0)
        {
            int randomMosquito = Random.Range(0, mosquitosList.Count);
            if (mosquitosList[randomMosquito].GetComponent<Mosquito>().SetLeader(true, 10))
            {
                numberOfLeaders--;
                leadersList.Add(mosquitosList[randomMosquito]);
            }
        }
    }
    #endregion
}
