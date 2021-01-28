using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaz de estado
/// Debe poder inicializarse, ejecutarse, procesar información y terminarse
/// La información se pasa de manera discreta (sólo en los cambios) ya que se produce con poca frecuencia normalmente
/// En caso de cambio de estado tras un proceso de información se debe devolver el nuevo estado
/// </summary>
public interface IState
{
    /// <summary>
    /// Inicializa el estado
    /// </summary>
    void InitState();

    /// <summary>
    /// Cada iteración swarm llamará a este método para ser controlado
    /// </summary>
    void ExecuteState();

    /// <summary>
    /// Procesa la información de cambio de visibilidad de la presa
    /// </summary>
    /// <param name="preyInSight"></param>
    /// <returns></returns>
    IState ProcessData(bool preyInSight);

    /// <summary>
    /// Procesa la información de un cambio de arma de la presa
    /// </summary>
    /// <param name="preyWeapon"></param>
    /// <returns></returns>
    IState ProcessData(Weapons preyWeapon);

    /// <summary>
    /// Procesa la información de un cambio en el número de mosquitos del flock
    /// </summary>
    /// <param name="mosquitosCount"></param>
    /// <returns></returns>
    IState ProcessData(int mosquitosCount);
    
    /// <summary>
    /// Finaliza el estado
    /// </summary>
    void EndState();
}
