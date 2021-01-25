using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : ISubState
{
    #region VARIABLES
    Swarm mySwarm;
    Transform swarmMovement;
    Vector2 currentVelocity;
    float timeElapsed;

    // Ajustes
    float maxTimeAttacking = 2f;
    // Ajustes de movimiento
    float dragAbsolute = 0.05f;
    float maxForce = 60f;
    float maxVelocity = 25f;
    #endregion

    public AttackingState(Swarm swarm)
    {
        mySwarm = swarm;
        swarmMovement = mySwarm.swarmMovement.transform;
        currentVelocity = Vector2.zero;
        timeElapsed = 0f;
    }

    #region INHERITED METHODS   
    public void InitState() {}

    public ISubState ExecuteState()
    {
        timeElapsed += Time.fixedDeltaTime;
        if (timeElapsed >= maxTimeAttacking)
            return new OrbitingState(mySwarm);

        ApplyDrag();
        ApplyAttackForce();
        ApplyCaps();
        swarmMovement.position = (Vector2)swarmMovement.position + currentVelocity * Time.fixedDeltaTime;

        return this;
    }

    void IState.ExecuteState()
    {
        throw new System.NotImplementedException();
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
    /// Aplica fuerza de ataque a currentVelocity
    /// </summary>
    private void ApplyAttackForce()
    {
        Vector2 force = mySwarm.GetPreyPosition() - (Vector2)mySwarm.transform.position;
        force = Vector2.ClampMagnitude(force, maxForce);
        force *= 1 / force.magnitude;
        currentVelocity += force;
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
