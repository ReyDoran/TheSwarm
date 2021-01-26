using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subestado de caza
/// Orbita al rededor de la presa
/// Sirve para darle descanso al jugador
/// </summary>
public class OrbitingState : ISubState
{
    #region VARIABLES
    Swarm mySwarm;
    Transform swarmMovement;
    Vector2 currentVelocity;
    float timeElapsed;
    // Ajustes elección de estado
    private int probabilityToAttack = 5;
    private int minBulletsToSurround = 5;
    // Ajustes
    float timeToChangeState = 3.5f; // Tiempo base para generar un cambio de estado
    float timeOffset = 2.0f;    // Tiempo a restar o añadir al anterior de manera aleatoria
    // Ajustes de movimiento
    float orbitRadius = 26f;    // Radio de la órbita (el enjambre se coloca en la mitad
    float orbitForce = 30f;     // Fuerza de órbita
    float orbitForceTangencial;
    float orbitForceTangencialMax = 2f;
    //float orbitForceTangencialMin = 0f;
    float maxVelocity = 15f;    // Velocidad máxima de órbita
    float minProximityPercentageTreshold = 0.1f;    // Rango en el que no se aplican fuerzas (desde la mitad hacia fuera y dentro)
    float dragAbsolute = 0.4f;  // Rozamiento
    #endregion
   
    public OrbitingState(Swarm swarm, float minIdleTime = 0f)
    {
        mySwarm = swarm;
        swarmMovement = mySwarm.swarmMovement.transform;
        timeElapsed = 0f;
        timeToChangeState += Random.Range(-timeOffset, timeOffset);
        timeToChangeState += minIdleTime;
        orbitForceTangencial = Random.Range(-orbitForceTangencialMax, orbitForceTangencialMax);
    }

    #region INHERITED METHODS
    public void InitState() 
    {
        if (mySwarm.isInFlames > 0)
            mySwarm.SetFormation(Formations.Disperse);
        else
            mySwarm.SetFormation(Formations.Standard);
        mySwarm.SetFlamesDefense(mySwarm.GetPrey().gameObject.GetComponent<PlayerMovementController>().drawnWeapon.Equals(Weapons.Flamethrower));
    }

    /// <summary>
    /// Calcula el nuevo vector de velocidad y se lo aplica a la posición
    /// </summary>
    public ISubState ExecuteState()
    {
        timeElapsed += Time.fixedDeltaTime;
        if (timeElapsed >= timeToChangeState)
        {
            if (mySwarm.GetBulletMosquitosCount() >= minBulletsToSurround)
            {
                if (Random.Range(0, probabilityToAttack) == 0)
                {
                    return new SurroundingState(mySwarm);
                }
                else
                {
                    return new AttackingState(mySwarm);
                }
            } else
            {
                return new AttackingState(mySwarm);
            }
        }

        ApplyDrag();
        ApplyOrbitForce();
        ApplyCaps();
        ApplyOrbitForceTangencial();
        swarmMovement.position = (Vector2)swarmMovement.position + currentVelocity * Time.fixedDeltaTime;

        if (mySwarm.isInFlames <= 3 && !mySwarm.GetPrey().gameObject.GetComponent<PlayerMovementController>().drawnWeapon.Equals(Weapons.Flamethrower))
            mySwarm.SetFormation(Formations.Standard);
        else
            mySwarm.SetFormation(Formations.Disperse);
        
        return this;
    }

    void IState.ExecuteState() 
    {
        throw new System.NotImplementedException();
    }

    public ISubState ProcessData(bool preyInSight)
    {
        throw new System.NotImplementedException();
    }

    public ISubState ProcessData(Weapons preyWeapon)
    {
        if (preyWeapon.Equals(Weapons.Flamethrower))
        {
            mySwarm.SetFlamesDefense(true);
            mySwarm.SetFormation(Formations.Disperse);
        }
        else
        {
            mySwarm.SetFlamesDefense(false);
        }
        return this;
    }

    public ISubState ProcessData(int mosquitosCount)
    {
        throw new System.NotImplementedException();
    }

    IState IState.ProcessData(bool preyInSight)
    {
        throw new System.NotImplementedException();
    }

    IState IState.ProcessData(Weapons preyWeapon)
    {
        throw new System.NotImplementedException();
    }

    IState IState.ProcessData(int mosquitosCount)
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

    private void ApplyOrbitForceTangencial()
    {
        Vector2 tangencialForce = Vector2.Perpendicular(mySwarm.GetFlockPosition() - mySwarm.GetPreyPosition()).normalized;
        currentVelocity += tangencialForce * orbitForceTangencial;
    }
    #endregion
}
