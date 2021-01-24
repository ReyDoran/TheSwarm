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
    public CircleCollider2D perceptionCircleCollider;   // Collider de la percepción
    private Transform transform;

    // Datos
    private List<GameObject> mosquitosList;    // Lista de agentes dentro del enjambre (de todos los tipos)
    private List<GameObject> leadersList;   // Lista de líderes
    private List<GameObject> phoenixMosquitosList;  // Lista de mosquitos fénix
    private Vector2 averagePosition;    // Posición media del cardumen
    private Formations formation;   // Formación del enjambre
    private Transform prey; // Presa del enjambre (jugador)
    private IState myState; // Estado actual

    private bool isReady;   // Está lista? (tiene el nº mínimo de agentes para considerarse flock)
    public bool isAnnexable;    // Se puede anexionar?
    public bool isPreyInSight;  // Tiene presa a la vista?

    // Ajustes
    private float swarmRadius = 10f;    // Radio del flock (no es el mismo el radio del flock que el radio para captar nuevos mosquitos para el flock)
    private float baseSwarmSize = 5f;  // Tamaño mínimo del flock
    private float swarmSizeDivisor = 40f;   // Factor de aumento de tamaño del flock por cada mosquito
    private float perceptionRadiusMultiplier = 3f;  // Multiplicador del radio de la percepción respecto al del enjambre
    private int minAgentsPerFlock = 10; // Mínimo de agentes para ser considerado flock
    private int annexableTreshold = 100;    // Máximo de agentes hasta convertirse en no anexionable

    // Controles para los estados
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
        formation = Formations.Standard;
        isReady = false;
        isAnnexable = true;
        isPreyInSight = false;
        swarmMovement = new GameObject();
        myState = new WanderingState(this);
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
            AddAgent(collision.gameObject);
        }
        else if (collision.gameObject.tag.Equals("Flock"))
        {
            if (this.GetInstanceID() > collision.gameObject.GetInstanceID())
            {
                Swarm otherFlock = collision.gameObject.GetComponent<Swarm>();
                if (GetAgentsCount() > otherFlock.GetAgentsCount() && otherFlock.isAnnexable)
                {
                    List<GameObject> otherFlockAgents = otherFlock.GetAllAgents();
                    while (otherFlockAgents.Count > 0)
                    {
                        AddAgent(otherFlock.RemoveAgent(otherFlockAgents[0]));
                    }
                }
                else if (isAnnexable)
                {
                    while (mosquitosList.Count > 0)
                    {
                        otherFlock.AddAgent(RemoveAgent(mosquitosList[0]));
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
        averagePosition = transform.position;
        if (mosquitosList.Count > 0)
        {
            foreach (GameObject agent in mosquitosList)
            {
                if (!agent.GetComponent<Mosquito>().isDead)
                {
                    averagePosition += new Vector2(agent.GetComponent<Transform>().position.x, agent.GetComponent<Transform>().position.y);
                }
            }
            averagePosition = averagePosition / mosquitosList.Count;
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
        if (destroyAuthorization && mosquitosList.Count < minAgentsPerFlock)
        {
            DestroyFlock();
        } else
        {
            swarmRadius = baseSwarmSize + mosquitosList.Count / swarmSizeDivisor;
            perceptionCircleCollider.radius = swarmRadius * perceptionRadiusMultiplier;
            circleCollider.radius = swarmRadius;
            foreach (GameObject agent in mosquitosList)
            {
                agent.GetComponent<Mosquito>().mySwarmRadius = swarmRadius;
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
        Debug.Log(newFormation);
        if (!formation.Equals(newFormation))
        {
            formation = newFormation;
            switch (formation)
            {
                case Formations.Standard:
                    foreach (GameObject agent in mosquitosList)
                    {
                        Mosquito mosquito = agent.GetComponent<Mosquito>();
                        mosquito.SetFormation(formation);
                    }
                    break;

                case Formations.Circle:
                    foreach (GameObject agent in mosquitosList)
                    {
                        Mosquito mosquito = agent.GetComponent<Mosquito>();
                        mosquito.SetFormation(formation);
                    }
                    break;

                case Formations.Disperse:
                    foreach (GameObject agent in mosquitosList)
                    {
                        Mosquito mosquito = agent.GetComponent<Mosquito>();
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
    /// Elimina agente del cardumen
    /// </summary>
    /// <param name="removedMosquito"></param>
    /// <returns></returns>
    public GameObject RemoveAgent(GameObject removedMosquito)
    {
        removedMosquito.GetComponent<Mosquito>().RemoveFlock();
        if (removedMosquito.TryGetComponent<PhoenixMosquito>(out PhoenixMosquito newPhoenixMosquito))
        {
            phoenixMosquitosList.Remove(newPhoenixMosquito.gameObject);
        }
        mosquitosList.Remove(removedMosquito);
        if (mosquitosList.Count < annexableTreshold)
            isAnnexable = true;
        ActualizeFlockRadius(true);
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
