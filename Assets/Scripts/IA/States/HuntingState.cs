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
    ISubState mySubState;
    ISubState mySubStateAux;
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
        mySubStateAux = mySubState;
        mySubState.InitState();
    }

    /// <summary>
    /// Ejecuta el subestado y comprueba posibles cambios de subestado
    /// </summary>
    public void ExecuteState()
    {
        CheckSubStateChange(mySubState.ExecuteState());
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
        CheckSubStateChange(mySubState.ProcessData(preyWeapon));
        return this;
    }

    public IState ProcessData(int mosquitosCount)
    {
        CheckSubStateChange(mySubState.ProcessData(mosquitosCount));
        return this;
    }

    /// <summary>
    /// Termina el subestado
    /// </summary>
    public void EndState()
    {
        mySubState.EndState();
    }
    #endregion

    #region PRIVATE METHODS
    /// <summary>
    /// Comprueba si ha habido un cambio de estado y ejecuta las órdenes necesarias en tal caso
    /// </summary>
    private void CheckSubStateChange(ISubState newState)
    {
        mySubState = newState;
        if (!mySubState.Equals(mySubStateAux))
        {
            mySubStateAux.EndState();
            mySubState.InitState();
            mySubStateAux = mySubState;
        }
    }
    #endregion
}
