using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estado de caza
/// Tiene subestados con los diferentes ataques o movimientos del enjambre
/// </summary>
public class HuntingState : IState
{
    #region VARIABLES
    Swarm mySwarm;
    IState mySubState;
    #endregion

    public HuntingState(Swarm swarm)
    {
        mySwarm = swarm;
    }

    #region INHERITED METHODS
    /// <summary>
    /// Inicializa el subestado de orbiting
    /// </summary>
    public void InitState()
    {
        mySubState = new OrbitingState(mySwarm);
        mySubState.InitState();
    }

    /// <summary>
    /// Ejecuta el subestado y comprueba posibles cambios de subestado
    /// </summary>
    public void ExecuteState()
    {
        mySubState.ExecuteState();
    }

    /// <summary>
    /// Si la presa se va de la vista cambia a wander
    /// </summary>
    /// <param name="preyInSight"></param>
    /// <returns></returns>
    public IState ProcessData(bool preyInSight)
    {
        if (!preyInSight)
        {
            return new WanderingState(mySwarm);
        }
        return this;
    }

    /// <summary>
    /// Si la presa utiliza el lanzallamas se defiende contra ello
    /// </summary>
    /// <param name="preyWeapon"></param>
    /// <returns></returns>
    public IState ProcessData(Weapons preyWeapon)
    {
        if (preyWeapon.Equals(Weapons.Flamethrower))
        {
            mySwarm.SetFlamesDefense(true);
            mySwarm.SetFormation(Formations.Disperse);
        }
        else
        {
            mySwarm.SetFlamesDefense(false);
            mySwarm.SetFormation(Formations.Standard);
        }
        return this;
    }

    public IState ProcessData(int mosquitosCount)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Termina el subestado
    /// </summary>
    public void EndState()
    {
        mySubState.EndState();
    }
    #endregion
}
