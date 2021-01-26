using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mosquito de tipo bala
/// Contiene método para lanzarse a por el objetivo
/// </summary>
public class BulletMosquito : Mosquito
{
    #region VARIABLES
    // Públicas
    public Material defaultMaterial;
    public Material lightedMaterial;

    // Datos
    private bool isAttacking;   // Está atacando el mosquito?
    private Transform parentAux;

    // Ajustes
    private float maxAttackForce = 25f; // Fuerza máxima de ataque
    private float attackDuration = 5.0f;
    private float attackForceMultiplier = 30f;  // Fuerza de ataque
    #endregion

    #region OVERRIDE METHODS
    private new void Awake()
    {
        base.Awake();
        isAttacking = false;
    }

    private new void OnEnable()
    {
        base.OnEnable();
        isAttacking = false;
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
        if (isBurning)
        {
            myFlame.GetComponent<Flame>().Extinguish();
            myAvoidZone.GetComponent<MosquitoAvoidZone>().ReturnToPool();
        }
        mainPool.AddBulletMosquito(gameObject);
        isDead = true;
        mainPool.RequestBlood(meshTransform.position);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Obtiene y aplica las fuerzas de flocking así como los limitadores
    /// </summary>
    protected override void ApplyFlockingForces()
    {
        myPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 flockingForce = new Vector2(0f, 0f);

        if (!isFlocking)
        {
            isAttacking = false;
        }

        // Obtiene las fuerzas  
        if (!isAttacking)
        {
            flockingForce += CalculateCohesionForce();
            flockingForce += CalculateRandomForce();
            if (avoidRequested == true)
            {
                avoidRequested = false;
                flockingForce += avoidForce;
            }
            Vector2.ClampMagnitude(flockingForce, maxForceAdded);
            // Aplica las fuerzas y el límite
            rigidbody.AddForce(flockingForce);
            ApplyCaps();
        } else
        {
            Vector2 force = mySwarm.GetPreyPosition() - (Vector2) transform.position;
            force *= 1 / force.magnitude;
            force *= attackForceMultiplier;
            force = Vector2.ClampMagnitude(force, maxAttackForce);
            flockingForce += force;
            rigidbody.AddForce(flockingForce);
        }
    }
    #endregion

    #region PUBLIC METHODS
    public void Attack()
    {
        if (!isAttacking)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = 0f;
        }
        isAttacking = true;
        parentAux = transform.parent;
        transform.SetParent(null);
        GetComponentInChildren<MeshRenderer>().material = lightedMaterial;
        Invoke(nameof(StopAttacking), attackDuration);
    }
    #endregion

    #region PRIVATE METHODS
    private void StopAttacking()
    {
        transform.SetParent(parentAux);
        GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
        isAttacking = false;
    }
    #endregion
}
