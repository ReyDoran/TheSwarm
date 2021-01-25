using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Almacena diferentes gameObjects desactivados y los devuelve activados cuando alguna clase se lo solicita (patrón de diseño optimización object pool)
/// Instancia X cantidad de cada uno al comenzar la escena y permite límites o nuevas instanciaciones en caso de sobrepasar las instancias disponibles
/// Las instancias se deben devolver al pool, no se hace automático
/// Actualmente almacena: mosquitos, flames, avoidZones y blood
/// </summary>
public class MainPool : MonoBehaviour
{
    #region VARIABLES
    // Referencias
    public GameObject mosquitoPrefab;
    public GameObject phoenixMosquitoPrefab;
    public GameObject bulletMosquitoPrefab;
    public GameObject flamePrefab;
    public GameObject avoidZonePrefab;
    public GameObject bloodPrefab;
    private UIController uIController;
    
    // Estructuras de datos
    private Stack<GameObject> mosquitosPool;
    private Stack<GameObject> phoenixMosquitosPool;
    private Stack<GameObject> bulletMosquitosPool;
    private Stack<GameObject> flamesPool;
    private Stack<GameObject> avoidZonesPool;
    private Stack<GameObject> bloodPool;

    // Pool inicial
    private int initialMosquitoInstances = 500;
    private int initialPhoenixMosquitoInstances = 50;
    private int initialBulletMosquitosInstances = 50;
    private int initialFlameInstances = 2;
    private int initialAvoidZoneInstances = 350;
    private int initialBloodInstances = 40;

    // Límites (no deben ser inferiores a los valores iniciales)
    private const int MAX_BLOOD_INSTANCES = 40;

    // Otros
    private Vector3 instantiatePosition = new Vector3(-10000f, -10000f, 0);
    #endregion VARIABLES

    #region UNITY CALLBACKS
    /// <summary>
    /// Inicializa las EDs con los valores dados
    /// </summary>
    private void Awake()
    {
        uIController = FindObjectOfType<UIController>();
        mosquitosPool = new Stack<GameObject>();
        phoenixMosquitosPool = new Stack<GameObject>();
        bulletMosquitosPool = new Stack<GameObject>();
        flamesPool = new Stack<GameObject>();
        avoidZonesPool = new Stack<GameObject>();
        bloodPool = new Stack<GameObject>();
        InitPool(initialMosquitoInstances, initialPhoenixMosquitoInstances, initialBulletMosquitosInstances, initialFlameInstances, initialAvoidZoneInstances, initialBloodInstances);
    }
    #endregion UNITY CALLBACKS

    #region PRIVATE METHODS
    /// <summary>
    /// Inicializa las pools con el número de instancias pasada como parámetros
    /// Las instancias estarán desactivadas y tendrán una referencia al pool
    /// </summary>
    /// <param name="numberOfMosquitos"></param>
    /// <param name="numberOfFlames"></param>
    /// <param name="numberOfAvoidZones"></param>
    /// <param name="numberOfBlood"></param>
    private void InitPool(int numberOfMosquitos, int numberOfPhoenixMosquitos, int numberOfBulletMosquitos, int numberOfFlames, int numberOfAvoidZones, int numberOfBlood)
    {
        for (int i = 0; i < numberOfMosquitos; i++)
        {
            GameObject newMosquito = Instantiate(mosquitoPrefab, instantiatePosition, Quaternion.identity);
            newMosquito.GetComponent<Mosquito>().mainPool = this;
            newMosquito.SetActive(false);
            mosquitosPool.Push(newMosquito);
        }
        for (int i = 0; i < numberOfPhoenixMosquitos; i++)
        {
            GameObject newPhoenixMosquito = Instantiate(phoenixMosquitoPrefab, instantiatePosition, Quaternion.identity);
            newPhoenixMosquito.GetComponent<Mosquito>().mainPool = this;
            newPhoenixMosquito.SetActive(false);
            phoenixMosquitosPool.Push(newPhoenixMosquito);
        }
        for (int i = 0; i < numberOfBulletMosquitos; i++)
        {
            GameObject newBulletMosquito = Instantiate(bulletMosquitoPrefab, instantiatePosition, Quaternion.identity);
            newBulletMosquito.GetComponent<Mosquito>().mainPool = this;
            newBulletMosquito.SetActive(false);
            bulletMosquitosPool.Push(newBulletMosquito);
        }
        for (int i = 0; i < numberOfFlames; i++)
        {
            GameObject newFlame = Instantiate(flamePrefab);
            newFlame.GetComponent<Flame>().mainPool = this;
            newFlame.SetActive(false);
            flamesPool.Push(newFlame);
        }
        for (int i = 0; i < numberOfAvoidZones; i++)
        {
            GameObject newAvoidZone = Instantiate(avoidZonePrefab);
            newAvoidZone.GetComponent<MosquitoAvoidZone>().mainPool = this;
            newAvoidZone.SetActive(false);
            avoidZonesPool.Push(newAvoidZone);
        }
        for(int i = 0; i < numberOfBlood; i++)
        {
            GameObject newBlood = Instantiate(bloodPrefab);
            newBlood.GetComponent<Blood>().mainPool = this;
            newBlood.SetActive(false);
            bloodPool.Push(newBlood);
        }
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Devuelve una lista con x mosquitos
    /// </summary>
    /// <param name="numberOfMosquitos"></param>
    /// <param name="mosquitos"></param>
    public void GetMosquitos(int numberOfMosquitos, out List<GameObject> mosquitos)
    {
        mosquitos = new List<GameObject>();
        for (int i = 0; i < numberOfMosquitos; i++)
        {
            mosquitos.Add(mosquitosPool.Pop());
        }
        for (int i = mosquitos.Count; i < numberOfMosquitos; i++)
        {
            GameObject newMosquito = Instantiate(mosquitoPrefab);
            newMosquito.GetComponent<Mosquito>().mainPool = this;
            mosquitos.Add(newMosquito);
        }
    }

    /// <summary>
    /// Devuelve un mosquito
    /// </summary>
    /// <returns></returns>
    public GameObject GetMosquito()
    {
        if (mosquitosPool.Count > 0)
        {
            return mosquitosPool.Pop();
        } else
        {
            GameObject newMosquito = Instantiate(mosquitoPrefab);
            newMosquito.GetComponent<Mosquito>().mainPool = this;
            return newMosquito;
        }
    }

    /// <summary>
    /// Devuelve un mosquito fénix
    /// </summary>
    /// <returns></returns>
    public GameObject GetPhoenixMosquito()
    {
        if (phoenixMosquitosPool.Count > 0)
        {
            return phoenixMosquitosPool.Pop();
        }
        else
        {
            GameObject newPhoenixMosquito = Instantiate(phoenixMosquitoPrefab);
            newPhoenixMosquito.GetComponent<PhoenixMosquito>().mainPool = this;
            return newPhoenixMosquito;
        }
    }

    /// <summary>
    /// Devuelve un mosquito bala
    /// </summary>
    /// <returns></returns>
    public GameObject GetBulletMosquito()
    {
        if (bulletMosquitosPool.Count > 0)
        {
            return bulletMosquitosPool.Pop();
        }
        else
        {
            GameObject newBulletMosquito = Instantiate(bulletMosquitoPrefab);
            newBulletMosquito.GetComponent<BulletMosquito>().mainPool = this;
            return newBulletMosquito;
        }
    }

    /// <summary>
    /// Devuelve una llama
    /// </summary>
    /// <returns></returns>
    public GameObject GetFlame()
    {
        if (flamesPool.Count > 0)
        {
            GameObject newFlame = flamesPool.Pop();
            newFlame.SetActive(true);
            newFlame.GetComponent<Flame>().enabled = true;
            return newFlame;
        }
        else
        {
            GameObject newFlame = Instantiate(flamePrefab);
            newFlame.GetComponent<Flame>().mainPool = this;
            return newFlame;
        }
    }

    /// <summary>
    /// Devuelve una avoidZone
    /// </summary>
    /// <returns></returns>
    public GameObject GetAvoidZone()
    {
        if (avoidZonesPool.Count > 0)
        {
            GameObject avoidZone = avoidZonesPool.Pop();
            avoidZone.SetActive(true);
            return avoidZone;
        }
        else
        {
            GameObject newAvoidZone = Instantiate(avoidZonePrefab);
            newAvoidZone.GetComponent<MosquitoAvoidZone>().mainPool = this;
            return newAvoidZone;
        }
    }

    /// <summary>
    /// Ejecuta una animación de explosión de sangre en la posición pasada como parámetro (si hay disponibles)
    /// </summary>
    public void RequestBlood(Vector3 position)
    {
        if (bloodPool.Count > 0)
        {
            GameObject blood = bloodPool.Pop();
            blood.SetActive(true);
            blood.transform.position = position;
        }
    }

    /// <summary>
    /// Desactiva y almacena el mosquito en el pool
    /// </summary>
    /// <param name="mosquito"></param>
    public void AddMosquito(GameObject mosquito)
    {
        mosquito.SetActive(false);
        mosquitosPool.Push(mosquito);
    }

    /// <summary>
    /// Desactiva y almacena el mosquito fénix en el pool
    /// </summary>
    /// <param name="mosquito"></param>
    public void AddPhoenixMosquito(GameObject phoenixMosquito)
    {
        phoenixMosquito.SetActive(false);
        phoenixMosquitosPool.Push(phoenixMosquito);
    }

    /// <summary>
    /// Desactiva y almacena el mosquito bala en el pool
    /// </summary>
    /// <param name="mosquito"></param>
    public void AddBulletMosquito(GameObject bulletMosquito)
    {
        bulletMosquito.SetActive(false);
        bulletMosquitosPool.Push(bulletMosquito);
    }

    /// <summary>
    /// Desactiva y almacena la flame en el pool
    /// </summary>
    /// <param name="flame"></param>
    public void AddFlame(GameObject flame)
    {
        flame.GetComponent<Flame>().enabled = false;
        flame.SetActive(false);
        flamesPool.Push(flame);
    }

    /// <summary>
    /// Desactiva y almacena la avoidZone en el pool
    /// </summary>
    /// <param name="avoidZone"></param>
    public void AddAvoidZone(GameObject avoidZone)
    {
        avoidZone.SetActive(false);
        avoidZonesPool.Push(avoidZone);
    }

    /// <summary>
    /// Desactiva y almacena la blood en el pool
    /// </summary>
    /// <param name="blood"></param>
    public void AddBlood(GameObject blood)
    {
        blood.SetActive(false);
        bloodPool.Push(blood);
    }

    public int FreeMosquitosCount()
    {
        return mosquitosPool.Count;
    }

    public int FreeFlamesCount()
    {
        return flamesPool.Count;
    }

    public int FreeAvoidZonesCount()
    {
        return avoidZonesPool.Count;
    }
    #endregion PUBLIC METHODS
}
