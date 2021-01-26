using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mosquito de tipo fénix, inmune a las llamas
/// Si se le ordena, añadirá a las fuerzas de flocking una en dirección a la presa
/// De esta manera defenderá al resto del enjambre de las llamas
/// </summary>
public class PhoenixMosquito : Mosquito
{
    #region VARIABLES
    private bool isFrontline;   // Forma parte de la primera línea del enjambre?
    private float frontlineForcePercentage = 0.8f; // Cuanto porcentaje valdrá respecto a cohesionForce
    private float frontlineForceModifier;
    //protected new float maxSpeed = 7.5f;

    private float randomForceReduction = 0.3f;
    private float cohesionForceReduction = 0.9f;
    #endregion

    #region OVERRIDES
    private new void OnEnable()
    {
        base.OnEnable();
        isFrontline = false;
    }

    /// <summary>
    /// Mata el mosquito, lo elimina del flock y lo desactiva
    /// </summary>
    public override void Kill()
    {
        if (isFlocking)
        {
            mySwarm.RemoveAgent(gameObject);
            isFlocking = false;
        }
        mainPool.AddPhoenixMosquito(gameObject);
        isDead = true;
        mainPool.RequestBlood(meshTransform.position);
        gameObject.SetActive(false);
    }

    public override bool Burn()
    {
        return false;
    }

    /// <summary>
    /// Override para las fuerzas de flocking
    /// Tiene en cuenta si hay que aplicar una fuerza hacia la frontline del enjambre
    /// </summary>
    protected override void ApplyFlockingForces()
    {
        myPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 flockingForce = new Vector2(0f, 0f);
        // Obtiene las fuerzas        
        flockingForce += CalculateCohesionForce() * cohesionForceReduction;
        flockingForce += CalculateRandomForce() * randomForceReduction;
        if (isFrontline)
            flockingForce += CalculateFrontlineForce();
        Vector2.ClampMagnitude(flockingForce, maxForceAdded);

        // Aplica las fuerzas y el límite
        rigidbody.AddForce(flockingForce);
        ApplyCaps();
    }

    /// <summary>
    /// Calcula una fuerza en dirección a la presa
    /// </summary>
    /// <returns></returns>
    private Vector2 CalculateFrontlineForce()
    {
        Vector2 frontlineForce = Vector2.zero;
        if (isFlocking && mySwarm.isPreyInSight)
        {
            frontlineForce = (mySwarm.GetPreyPosition() - (Vector2)transform.position).normalized * frontlineForceModifier;
        }        
        return frontlineForce;
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Setea si debe formar parte de la frontline del enjambre
    /// </summary>
    /// <param name="frontline"></param>
    public void SetFrontline(bool frontline)
    {
        isFrontline = frontline;
        frontlineForceModifier = cohesionForceModifier * frontlineForcePercentage;
    }
    #endregion
}
