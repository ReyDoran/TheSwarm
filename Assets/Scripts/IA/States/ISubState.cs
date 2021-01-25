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
}
