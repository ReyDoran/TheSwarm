using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Devuelve el sistema de partículas al pool al detenerse
/// </summary>
public class Blood : MonoBehaviour
{
    #region VARIABLES
    public MainPool mainPool;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    /// <summary>
    /// Cuando se termina la animación, se devuelve al pool
    /// </summary>
    private void OnParticleSystemStopped()
    {
        mainPool.AddBlood(gameObject);
    }
    #endregion
}
