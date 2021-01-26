using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class SurroundingState : ISubState
{
    #region VARIABLES
    // Referencias
    Swarm mySwarm;
    Transform swarmMovement;
    MosquitoAvoidZone preyAvoidZone;

    // Datos
    private bool isReady;   // Indica si ya se ha rodeado a la presa y puede empezara atacar
    private bool isFinished;
    private Vector2 currentVelocity;
    Timer startTimer;
    private float timeElapsedAttack;    // Tiempo desde el último ataque
    private float timeElapsed;  // Tiempo desde el inicio del estado

    // Ajustes
    private float minBulletsToExecute = 5;
    private float maxDuration = 13.0f;
    private float preyAvoidZoneRadius = 5.2f;
    private float preyAvoidZoneForce = 3f;
    float dragAbsolute = 0.6f;
    float maxForce = 18f;
    float maxVelocity = 25f;
    float distanceToStart = 2.5f;   // Distancia a la presa del flock para considerar el comienzo del ataque

    // Ajustes ataques
    private int maxMosquitosPerAttack = 3;
    private float timeToNextAttack = 0f;
    private float timeBetweenAttacks = 2.0f;    // Tiempo base entre ataques
    private float timeOffsetAttacks = 1.0f;    // Cantidad que puede variar (aleatoriamente) el tiempo entre ataques
    #endregion

    public SurroundingState(Swarm swarm)
    {
        mySwarm = swarm;
        swarmMovement = mySwarm.swarmMovement.transform;
        preyAvoidZone = Object.FindObjectOfType<MainPool>().GetAvoidZone().GetComponent<MosquitoAvoidZone>();
        preyAvoidZone.GetComponent<CircleCollider2D>().radius = preyAvoidZoneRadius;
        currentVelocity = Vector2.zero;
        isReady = false;
        isFinished = false;
        timeElapsed = 0f;
        timeElapsedAttack = 0f;
        timeToNextAttack = timeBetweenAttacks + Random.Range(-timeOffsetAttacks, timeOffsetAttacks);
    }

    #region INHERITED METHODS
    public void InitState() 
    {
        preyAvoidZone.SetTarget(mySwarm.GetPrey(), preyAvoidZoneForce);
        mySwarm.SetFormation(Formations.Circle);
    }

    public ISubState ExecuteState()
    {
        if (!isReady)
        {
            // Rodea al enemigo
            Surround();
        } else
        {
            // Frena si está en movimiento
            dragAbsolute = 0.2f;
            ApplyDrag();
            swarmMovement.position = (Vector2)swarmMovement.position + currentVelocity * Time.fixedDeltaTime;

            // Ataca
            timeElapsedAttack += Time.fixedDeltaTime;
            if (timeElapsedAttack > timeToNextAttack)
            {
                timeElapsedAttack = 0;
                timeToNextAttack = timeBetweenAttacks + Random.Range(-timeOffsetAttacks, timeOffsetAttacks);
                mySwarm.FireBulletMosquito(Random.Range(0, maxMosquitosPerAttack) + 1);
            }

            // Comprueba si debe terminar por tiempo
            timeElapsed += Time.fixedDeltaTime;
            if(timeElapsed >= maxDuration)
            {
                return new OrbitingState(mySwarm, 4.0f);
            }

            // Comprueba si debe terminar por enemigo fuera del círculo
            if (Vector2.Distance(mySwarm.GetPreyPosition(), mySwarm.GetFlockPosition()) > mySwarm.GetRadius())
            {
                return new OrbitingState(mySwarm);
            }

            // Comprueba si debe terminar por pocos mosquitos bala
            if (mySwarm.GetBulletMosquitosCount() < minBulletsToExecute)
            {
                return new OrbitingState(mySwarm);
            }
        }
        return this;
    }

    void IState.ExecuteState()
    {
        throw new System.NotImplementedException();
    }

    IState IState.ProcessData(bool preyInSight)
    {
        throw new System.NotImplementedException();
    }

    IState IState.ProcessData(Weapons preyWeapon)
    {
        throw new System.NotImplementedException();
    }

    IState IState.ProcessData(int mosquitosCount)
    {
        throw new System.NotImplementedException();
    }


    public ISubState ProcessData(bool preyInSight)
    {
        throw new System.NotImplementedException();
    }

    public ISubState ProcessData(Weapons preyWeapon) 
    {
        return this;
    }

    public ISubState ProcessData(int mosquitosCount)
    {
        throw new System.NotImplementedException();
    }

    public void EndState()
    {
        isFinished = true;
        if (preyAvoidZone != null)
        {
            preyAvoidZone.ReturnToPool();
        }
        preyAvoidZone = Object.FindObjectOfType<MainPool>().GetAvoidZone().GetComponent<MosquitoAvoidZone>();
        preyAvoidZone.GetComponent<CircleCollider2D>().radius = preyAvoidZoneRadius;
        preyAvoidZone.SetTarget(mySwarm.GetPrey(), 7f);
        preyAvoidZone.ReturnToPoolDelayed(5.0f);
    }
    #endregion

    #region PRIVATE METHODS
    private void Surround()
    {        
        // Persigue a la presa
        ApplyDrag();
        ApplyAttackForce();
        ApplyCaps();
        swarmMovement.position = (Vector2)swarmMovement.position + currentVelocity * Time.fixedDeltaTime;

        // Si está encima pasa al siguiente estado
        if (Vector2.Distance(mySwarm.transform.position, mySwarm.GetPreyPosition()) < distanceToStart)
        {
            if (startTimer == null)
            {
                startTimer = new Timer(3000);
                startTimer.Elapsed += new ElapsedEventHandler(SetReadyToTrue);
                startTimer.Enabled = true;
            }
            if (!isFinished)
                preyAvoidZone.ReturnToPool();
        }
    }

    private void SetReadyToTrue(object source, ElapsedEventArgs e)
    {
        isReady = true;
    }

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
