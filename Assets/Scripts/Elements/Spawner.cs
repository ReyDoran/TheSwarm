using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generador de mosquitos (CLASE PARA TESTING)
/// Obtiene de la pool mosquitos y los instancia de diversas maneras
/// También puede instanciar enjambres
/// </summary>
public class Spawner : MonoBehaviour
{
    #region VARIABLES
    // Referencias
    public GameObject swarm;    // Prefab de enjambre    
    public GameObject mosquito; // Prefab mosquito
    public int mosquitoFrecuency;   // Frecuencia de aparición
    public GameObject phoenixMosquito;  // Prefab mosquito fénix
    public int phoenixMosquitoFrecuency;

    public MainPool mainPool;   // Pool de la escena


    // Ajustes
    public int numberOfInstances;   // Número de mosquitos a generar al ser creado


    // Auxiliares
    private float spawnTimer;   // Almacena el tiempo pasado
    private int[] frecuencies;


    // TEST
    public int formationIndex;
    #endregion VARIABLES

    #region UNITY CALLBACKS
    private void Awake()
    {
        spawnTimer = 0f;
        frecuencies = new int[2];
        frecuencies[0] = mosquitoFrecuency;
        frecuencies[1] = phoenixMosquitoFrecuency;
    }

    // Para testeo
    void Start()
    {
        SpawnMosquitosDelayed(numberOfInstances);
        Invoke("ActivateFlock", 1f);
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 5)
        {
            spawnTimer = 0;
            //SpawnMosquito();
        }
    }
    #endregion

    #region PRIVATE METHODS
    private void SpawnMosquito(Vector2 position, float maxOffsetX, float maxOffsetY)
    {
        float offsetX = Random.Range(-maxOffsetX / 2, maxOffsetX / 2);
        float offsetY = Random.Range(-maxOffsetY / 2, maxOffsetY / 2);
        Mosquito mosquito = GetRandomMosquito().GetComponent<Mosquito>();
        mosquito.gameObject.SetActive(true);
        mosquito.transform.position = new Vector3(position.x + offsetX, position.y + offsetY, 0);
        mosquito.isDead = false;
    }

    private GameObject GetRandomMosquito()
    {
        int total = mosquitoFrecuency + phoenixMosquitoFrecuency;
        int randomIndex = Random.Range(0, total) + 1;
        int iteration = -1;
        do
        {
            iteration++;
            randomIndex -= frecuencies[iteration];
        } while (randomIndex > 0);
        GameObject mosquito;
        switch(iteration)
        {
            default:
            case 0:
                mosquito = mainPool.GetMosquito();
                break;
            case 1:
                mosquito = mainPool.GetPhoenixMosquito();
                break;
        }
        return mosquito;
    }
    #endregion

    #region TEST METHODS
    private void SpawnMosquitosTest()
    {
        SpawnMosquito(transform.position, 1f, 1f);
    }

    // Spawnea
    private void SpawnMosquitosDelayed(int number)
    {
        for (int i = 0; i < number; i++)
        {
            Invoke(nameof(SpawnMosquitosTest), 0.0025f * i);
        }
    }

    // Crea un flock en la posición en la que se encuentra
    private void ActivateFlock()
    {
        Debug.Log(transform.position);
        GameObject newSwarm = Instantiate(swarm, transform.position, Quaternion.identity);
        newSwarm.GetComponent<Swarm>().SetFormation((Formations)formationIndex);
    }
    #endregion
}
