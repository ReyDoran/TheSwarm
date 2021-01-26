using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estado de vagar
/// Estado inicial
/// Tiene movimiento aleatorio
/// </summary>
public class WanderingState : IState
{
    #region VARIABLES
    Swarm mySwarm;
    Transform swarmMovement;
    Vector2 direction;  // Dirección actual
    float speed = 5f;   // Velocidad de wander
    #endregion

    public WanderingState(Swarm swarm)
    {
        mySwarm = swarm;
        direction = Vector2.zero;
        swarmMovement = mySwarm.swarmMovement.transform;
    }

    #region INHERITED METHODS
    public void InitState() {}

    /// <summary>
    /// Simulación de movimiento wander
    /// </summary>
    public void ExecuteState()
    {
        direction += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.2f;
        direction.Normalize();
        swarmMovement.position = (Vector2)swarmMovement.position + direction * speed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Si ve una presa cambia a estado Hunting
    /// </summary>
    /// <param name="preyInSight"></param>
    /// <returns></returns>
    public IState ProcessData(bool preyInSight)
    {
        if (preyInSight)
        {
            return new HuntingState(mySwarm); 
        }
        return this;
    }

    public IState ProcessData(Weapons preyWeapon)
    {
        return new HuntingState(mySwarm);
    }

    public IState ProcessData(int mosquitosCount)
    {
        return this;
    }

    public void EndState() { }
    #endregion
}
