using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subestado de caza
/// Orbita al rededor de la presa
/// Sirve para darle descanso al jugador
/// </summary>
public class OrbitingState : IState
{
    #region VARIABLES
    Swarm mySwarm;
    Transform swarmMovement;
    Vector2 currentVelocity;

    // Ajustes de movimiento
    float orbitRadius = 26f;    // Radio de la órbita (el enjambre se coloca en la mitad
    float orbitForce = 30f;     // Fuerza de órbita
    float maxVelocity = 15f;    // Velocidad máxima de órbita
    float minProximityPercentageTreshold = 0.1f;    // Rango en el que no se aplican fuerzas (desde la mitad hacia fuera y dentro)
    float dragAbsolute = 0.4f;  // Rozamiento
    #endregion
   
    public OrbitingState(Swarm swarm)
    {
        mySwarm = swarm;
        swarmMovement = mySwarm.swarmMovement.transform;
    }

    #region INHERITED METHODS
    public void InitState() {}

    /// <summary>
    /// Calcula el nuevo vector de velocidad y se lo aplica a la posición
    /// </summary>
    public void ExecuteState()
    {
        ApplyDrag();
        ApplyOrbitForce();
        ApplyCaps();

        swarmMovement.position = (Vector2)swarmMovement.position + currentVelocity * Time.fixedDeltaTime;
    }

    public IState ProcessData(bool preyInSight)
    {
        throw new System.NotImplementedException();
    }

    public IState ProcessData(Weapons preyWeapon)
    {
        throw new System.NotImplementedException();
    }

    public IState ProcessData(int mosquitosCount)
    {
        throw new System.NotImplementedException();
    }

    public void EndState() {}
    #endregion

    #region PRIVATE METHODS
    /// <summary>
    /// Aplica rozamiento a currentVelocity
    /// </summary>
    private void ApplyDrag()
    {
        currentVelocity = Vector2.ClampMagnitude(currentVelocity, currentVelocity.magnitude - dragAbsolute);
    }

    /// <summary>
    /// Aplica fuerza de órbita a currentVelocity
    /// </summary>
    private void ApplyOrbitForce()
    {
        Vector2 force = mySwarm.GetPreyPosition() - (Vector2)mySwarm.transform.position;
        float proximity = Mathf.Clamp(force.magnitude / orbitRadius, 0f, 1f);
        proximity -= 0.5f;
        if (Mathf.Abs(proximity) < minProximityPercentageTreshold)
            proximity = 0f;
        force.Normalize();
        force *= proximity;
        currentVelocity += force * orbitForce;
    }

    /// <summary>
    /// Aplica el limitador de velocidad
    /// </summary>
    private void ApplyCaps()
    {
        currentVelocity = Vector2.ClampMagnitude(currentVelocity, maxVelocity);
    }    
    #endregion
}
