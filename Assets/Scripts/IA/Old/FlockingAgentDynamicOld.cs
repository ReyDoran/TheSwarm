using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgentDynamicOld : FlockingAgentOld
{
    // Referencias
    private PerceptionOld flockPerception;    // Controlador de percepción
    private Rigidbody2D rigidbody;
    private Transform meshTransform;

    // Variables de información
    private Vector2 myPosition; // Almacena la posición
    private bool flyingUpwards; // Almacena la dirección en el eje Z en la que se está moviendo

    // Ajustes de flocking
    private float cohesionStrength = 30f;   // Fuerza de cohesión (15 default)
    private float separationStrength = 6f;  // Fuerza de separación
    private float separationTreshold = 0.7f;   // Distancia que se considera demasiado cerca
    private float minDist = 0.5f;   // Distancia mínima entre dos agentes
    private float randomStrength = 12f; // Fuerza aleatoria
    private float randomStrengthZ = 0.03f;  // Fuerza aleatoria en el eje z
    private float minPositionZ = -3f;    // Posición máxima del mosquito en el eje z (-2 default)
    private float maxPositionZ = -4f;   // Posición mínima del mosquito en el eje z (-5 default)


    // Start is called before the first frame update
    void Start()
    {
        flockPerception = GetComponentInChildren<PerceptionOld>();
        rigidbody = GetComponent<Rigidbody2D>();
        meshTransform = GetComponentInChildren<MeshRenderer>().transform;
        flyingUpwards = true;
    }

    /*
     * Calcula la fuerza de cohesión en función de la posición media del cardumen.
     * No tiene en cuenta el nº de agentes en el cardumen (debería?)
     */
    private Vector2 CalculateCohesionForce(Vector2 flockPosition)
    {
        // Calcula dirección
        Vector2 finalForce = flockPosition - myPosition;
        float distance = finalForce.magnitude;
        finalForce.Normalize();
        finalForce *= Mathf.Clamp(distance / flockPerception.GetComponent<CircleCollider2D>().radius + 0.5f, 0f, 1f);   // Aplica un porcentaje dependiendo de lo lejos que esté del centro del cardumen       
        finalForce *= cohesionStrength; // Aplica potencia
        return finalForce;
    }

    /*
     * Calcula la fuerza de separación en función respecto a los agentes pasados como parámetro.
     * Tiene en cuenta lo cerca que está aún dentro del treshold de separación.
     */
    private Vector2 CalculateSeparationForce(List<GameObject> closeAgentsList)
    {
        Vector2 returnForce = new Vector2(0, 0);
        foreach (GameObject agent in closeAgentsList)   // Calcula la dirección de la fuerza de separación entre todos los agentes cercanos
        {
            Vector2 agentPosition = new Vector2(agent.transform.position.x, agent.transform.position.y);
            Vector2 partialForce = new Vector2(0, 0);
            partialForce += (myPosition - agentPosition);
            partialForce.Normalize();
            partialForce *= 1 - (Vector2.Distance(myPosition, agent.transform.position) - minDist) / separationTreshold;    // Aplica un porcentaje dependiendo de lo cerca que esté
            returnForce += partialForce;
        }
        returnForce *= separationStrength;   // Aplica la potencia
        return returnForce;
    }

    /*
     * Calcula una fuerza aleatoria
     */
    private Vector2 CalculateRandomForce()
    {
        Vector2 returnForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        returnForce *= randomStrength;
        return returnForce;
    }

    /*
     * Lee los valores de la percepción, llama a las funciones de calcular fuerzas y las aplica.
     */
    private void ApplyFlockingForces()
    {
        Vector2 linearForce = new Vector2(0f, 0f);
        List<GameObject> agentsList = flockPerception.GetAllAgents();   // Lista completa de agentes en el campo de visión
        List<GameObject> closeAgentsList = new List<GameObject>();  // Agentes demasiado cercanos
        Vector2 flockPosition = new Vector2(0, 0);  // Posición media del cardumen
        bool isFlocking = agentsList.Count > 0; // Informa de si hay enjambre
        myPosition.x = rigidbody.position.x;    // Actualiza la información de posición
        myPosition.y = rigidbody.position.y;    // Actualiza la información de posición

        // Calcula la posición y ángulo medio y la lista de agentes demasiado cercanos
        foreach (GameObject agent in agentsList)
        {
            Vector2 agentPosition = new Vector2(agent.transform.position.x, agent.transform.position.y);
            flockPosition += agentPosition;
            if (Vector2.Distance(myPosition, agentPosition) < separationTreshold)
            {
                closeAgentsList.Add(agent);
            }
        }
        flockPosition /= agentsList.Count;

        // Calcula y aplica las fuerzas.
        if (isFlocking)
        {
            linearForce += CalculateCohesionForce(flockPosition);
            linearForce += CalculateSeparationForce(closeAgentsList);
        }        
        linearForce += CalculateRandomForce();
        rigidbody.AddForce(linearForce);
    }

    /*
     *  Aplica fuerza en el eje Z para que se mueva aleatorio dentro de unos valores (minPositionZ, maxPositionZ)
     *  No se tiene en cuenta las fuerzas de flocking, ya que estas sólo aplican en el plano x,y.
     */
    private void ApplyRandomZ()
    {
        float distanceTreshold = 1.5f;  // Distancia de los límites a la que se empieza a aplicar fuerza para evitar que se salga
        float forceZ;
        if (meshTransform.position.z > minPositionZ + distanceTreshold)
        {
            forceZ = - randomStrengthZ;
            flyingUpwards = true;
        } else if (meshTransform.position.z < maxPositionZ - distanceTreshold)
        {
            forceZ = randomStrengthZ;
            flyingUpwards = false;
        } else
        {
            if (Random.Range(0f, 1f) > 0.98f)
            {
                flyingUpwards = !flyingUpwards;
            }
            if (flyingUpwards == true)
            {
                forceZ = - randomStrengthZ;
            } else
            {
                forceZ = randomStrengthZ;
            }
        }
        meshTransform.position = new Vector3(meshTransform.position.x, meshTransform.position.y, meshTransform.position.z + forceZ);
    }

    private void FixedUpdate()
    {
        ApplyFlockingForces();
        ApplyRandomZ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
