using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Similar a un estado excepto por que una ejecución puede resultar en un cambio de estado
/// </summary>
public interface ISubState : IState
{
    /// <summary>
    /// Ejecuta el estado devolviendo un nuevo estado en caso de evento de cambio
    /// </summary>
    /// <returns></returns>
    new ISubState ExecuteState();

    /// <summary>
    /// Procesa la información de cambio de visibilidad de la presa
    /// </summary>
    /// <param name="preyInSight"></param>
    /// <returns></returns>
    new ISubState ProcessData(bool preyInSight);

    /// <summary>
    /// Procesa la información de un cambio de arma de la presa
    /// </summary>
    /// <param name="preyWeapon"></param>
    /// <returns></returns>
    new ISubState ProcessData(Weapons preyWeapon);

    /// <summary>
    /// Procesa la información de un cambio en el número de mosquitos del flock
    /// </summary>
    /// <param name="mosquitosCount"></param>
    /// <returns></returns>
    new ISubState ProcessData(int mosquitosCount);
}
