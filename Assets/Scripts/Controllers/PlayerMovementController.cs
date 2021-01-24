using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controla el movimiento, el estado y las acciones del personaje jugable.
/// Controla las animaciones y el sonido asociados.
/// Lee el input del jugador y aplica el movimiento y los ataques.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    #region VARIABLES
    // Publicos
    public GameObject grenadePrefab;
    public UnityEvent weaponSwapEvent;

    
    // Referencias
    private CircleCollider2D circleCollider;
    private Rigidbody2D rigidbody;
    private MainPool mainPool;

    public Camera camera;
    private Actions actions;
    private PlayerController playerController;

    private InputController input;
    private PlayerSoundController soundController;
    private UIController uiController;       


    // Input
    private Vector2 inputValueMovement; // eje 2D (movimiento)
    private Vector2 inputValueAim;  // eje 2D (posición de apuntar)

  
    // Info del personaje
    private int health; // Salud (0 - 100)
    private bool isDead;    // Ha muerto el jugador?
    private Vector2 aimDirection;    // Dirección de apuntar (x, y) normalizada
    private float aimDistance;  // Distancia de apuntar
    public Weapons drawnWeapon;    // Arma en uso
    private float timeSinceLastShot;    // Tiempo desde el último disparo
    private bool pullingTrigger;    // Está el jugador con el gatillo pulsado?
    private bool weaponChangeRequested; // Se ha pedido cambio de arma? (ANIMACIONES)
    private bool isDrawnWeaponAutomatic;    // Es automática el arma en uso?
    public int[] ammunition;    // Munición disponible de cada arma (orden enum Weapons)
    private float drawnWeaponCooldown;  // Cooldown del arma en uso


    // Ajustes de personaje
    private float movementModifier = 17f;   // Modificador de movimiento
    private float hitboxRadius = 0.1f;  // Radio de la hitbox
    // Cooldowns
    private float pistolCooldown = 0.5f;
    private float rifleCooldown = 0.2f;
    private float flamethrowerCooldown = 0.05f;
    private float grenadeLauncherCooldown = 2f;
    private float sniperCooldown = 1.5f;
    // Velocidades de proyectiles
    private float flamesVelocity = 24f;
    private float grenadeVelocity = 24f;
    // Otros
    private float shotRange = 35f;  // Alcance máximo de las balas
    private float shootAnimationTime = 0.25f;   // Tiempo que se ejecuta la animación de disparar
    private float projectileHeight = -4f;   // Altura en z de los proyectiles (llamas y cohete)
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        input = new InputController();
        rigidbody = GetComponent<Rigidbody2D>();
        actions = GetComponent<Actions>();
        playerController = GetComponent<PlayerController>();
        playerController.arsenal[0].rightGun.transform.localScale = Vector3.one * 2;    // Ajustamos tamaño de las armas
        soundController = GetComponent<PlayerSoundController>();
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = hitboxRadius;
        isDead = false;
        timeSinceLastShot = Time.time;
        health = 100;
        drawnWeapon = Weapons.Pistol;
        isDrawnWeaponAutomatic = false;
        ammunition = new int[4];
        drawnWeaponCooldown = GetWeaponCooldown(drawnWeapon);
    }

    void Start()
    {
        uiController = FindObjectOfType<UIController>();
        mainPool = FindObjectOfType<MainPool>();

        // Callbacks del input
        input.Basic.Movement.performed += context => { inputValueMovement = input.Basic.Movement.ReadValue<Vector2>(); };
        input.Basic.Movement.canceled += context => { inputValueMovement = input.Basic.Movement.ReadValue<Vector2>(); };
        input.Basic.AimMouse.performed += context => { CalculateAimDirection(); };
        input.Basic.AimController.performed += context => { CalculateAimDirectionController(); };
        input.Basic.Shoot.performed += context => { SetTrigger(true); };
        input.Basic.Shoot.canceled += context => { SetTrigger(false); };
        input.Basic.SwapWeapon1.performed += context => { SetDrawnWeapon((Weapons)0); };
        input.Basic.SwapWeapon2.performed += context => { SetDrawnWeapon((Weapons)1); };
        input.Basic.SwapWeapon3.performed += context => { SetDrawnWeapon((Weapons)2); };
        input.Basic.SwapWeapon4.performed += context => { SetDrawnWeapon((Weapons)3); };
        input.Basic.SwapWeapon5.performed += context => { SetDrawnWeapon((Weapons)4); };
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    /// <summary>
    /// Si colisiona con un mosquito, lo mata y se aplica el daño que produzca
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Mosquito"))
        {
            Mosquito mosquito = collider.GetComponent<Mosquito>();
            ApplyDamage(mosquito.damage);
            mosquito.Kill();
        }
    }

    /// <summary>
    /// Aplica las fuerzas de movimiento del personaje
    /// </summary>
    private void FixedUpdate()
    {
        ApplyForce();
    }

    /// <summary>
    /// Rota al personaje a la posición en la que apunta
    /// Ejecuta las animaciones
    /// Decide si se debe disparar el arma
    /// Cambia el arma si ha sido solicitado (ANIMACIONES)
    /// </summary>
    private void Update()
    {
        AimAt();

        PlayAnimations();

        timeSinceLastShot += Time.deltaTime;
        if (pullingTrigger)
        {
            if (timeSinceLastShot > drawnWeaponCooldown)
            {
                Shoot();
                if (!isDrawnWeaponAutomatic)
                {
                    pullingTrigger = false;
                }
            }
        }

        if (weaponChangeRequested)
        {
            weaponChangeRequested = false;
            playerController.SetArsenal(drawnWeapon.ToString());
        }
    }
    #endregion

    #region PRIVATE METHODS
    /// <summary>
    /// Asigna el valor a pullingTrigger
    /// Si se está usando el flamethrower, detiene el sonido en caso de levantar el gatillo
    /// </summary>
    /// <param name="pulling"></param>
    private void SetTrigger(bool pulling)
    {
        pullingTrigger = pulling;
        if (pulling == false && drawnWeapon == Weapons.Flamethrower)
            soundController.StopWeaponShot(drawnWeapon);
    }

    /// <summary>
    /// Ejecuta los eventos de fin de partida:
    /// - Desactiva el jugador
    /// - Ejecuta la animación y sonido de muerte
    /// - Activa el menú de game over
    /// </summary>
    public void Die()
    {
        isDead = true;
        circleCollider.enabled = false;
        rigidbody.velocity = Vector2.zero;
        soundController.PlayDeathSound();
        actions.Death();
        uiController.GameOver();
        this.enabled = false;
    }

    /// <summary>
    /// Calcula y almacena en aimDirection la dirección en la que está apuntando el jugador
    /// Calcula y almacena la distancia al punto al que se está apuntando
    /// Dirección entre jugador y posición del ratón normalizada
    /// </summary>
    private void CalculateAimDirection()
    {
        inputValueAim = input.Basic.AimMouse.ReadValue<Vector2>();
        Ray cameraRay = camera.ScreenPointToRay(inputValueAim);
        Plane groundPlane = new Plane(new Vector3(0, 0, -3), Vector3.zero);
        if (groundPlane.Raycast(cameraRay, out float rayLength))
        {
            Vector2 aimPosition = cameraRay.GetPoint(rayLength);
            aimDirection = aimPosition - new Vector2(transform.position.x, transform.position.y);
            aimDistance = aimDirection.magnitude;
            aimDirection.Normalize();
        }
    }

    /// <summary>
    /// Calcula y almacena en aimDirection la dirección en la que está apuntando el jugador (modo mando)
    /// Calcula y almacena la distancia al punto al que se está apuntando
    /// Dirección entre jugador y posición del ratón normalizada (simulada a través del joystick)
    /// </summary>
    private void CalculateAimDirectionController()
    {
        inputValueAim = input.Basic.AimMouse.ReadValue<Vector2>();
        inputValueAim.x = inputValueAim.x * Screen.width;
        inputValueAim.y = inputValueAim.y * Screen.height;
        Debug.Log(inputValueAim);
        Ray cameraRay = camera.ScreenPointToRay(inputValueAim);
        Plane groundPlane = new Plane(new Vector3(0, 0, -3), Vector3.zero);
        if (groundPlane.Raycast(cameraRay, out float rayLength))
        {
            Vector2 aimPosition = cameraRay.GetPoint(rayLength);
            aimDirection = aimPosition - new Vector2(transform.position.x, transform.position.y);
            aimDistance = aimDirection.magnitude;
            aimDirection.Normalize();
        }
    }

    /// <summary>
    /// Orienta al personaje hacia la dirección en la que apunta.
    /// </summary>
    private void AimAt()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(aimDirection.x, aimDirection.y, 0), new Vector3(0, 0, -1));
        
    }

    /// <summary>
    /// Cambia el arma en uso a la pasada como parámetro.
    /// Ajusta los diferentes parámetros y datos en función de cual es
    /// </summary>
    /// <param name="weapon"></param>
    private void SetDrawnWeapon(Weapons weapon)
    {
        drawnWeapon = weapon;
        weaponChangeRequested = true;   // (ANIMACION)
        if (weapon != Weapons.Flamethrower)
            soundController.StopWeaponShot(Weapons.Flamethrower);
        switch(drawnWeapon)
        {
            case Weapons.Pistol:
                isDrawnWeaponAutomatic = false;
                drawnWeaponCooldown = pistolCooldown;
                break;

            case Weapons.Rifle:
                isDrawnWeaponAutomatic = true;
                drawnWeaponCooldown = rifleCooldown;
                break;

            case Weapons.Flamethrower:
                isDrawnWeaponAutomatic = true;
                drawnWeaponCooldown = flamethrowerCooldown;
                break;

            case Weapons.GrenadeLauncher:
                isDrawnWeaponAutomatic = false;
                drawnWeaponCooldown = grenadeLauncherCooldown;
                break;

            case Weapons.Sniper:
                isDrawnWeaponAutomatic = false;
                drawnWeaponCooldown = sniperCooldown;
                break;
        }
        weaponSwapEvent.Invoke();
    }

    /// <summary>
    /// Dispara con el arma que tenga desenfundada.
    /// Dependiendo del arma que sea:
    /// - Utiliza raycast y llama a los efectos deseados en los objetos colisionados (matar mosquitos)
    /// - Instancia/obtiene un proyectil y le comunica la dirección
    /// Ademas, pide que se reproduzca el sonido y resetea el tiempo para el cálculo del cooldown.
    /// </summary>
    private void Shoot()
    {
        timeSinceLastShot = 0;
        actions.Attack();
        soundController.PlayWeaponShot(drawnWeapon);
        int layerMask = LayerMask.GetMask("Mosquitoes");
        switch (drawnWeapon)
        {
            case Weapons.Pistol:               
                RaycastHit2D raycastPistolHit2D = Physics2D.Raycast(transform.position, aimDirection, shotRange, layerMask);
                if (raycastPistolHit2D.collider != null && raycastPistolHit2D.collider.gameObject.CompareTag("Mosquito"))
                {
                    raycastPistolHit2D.rigidbody.gameObject.GetComponent<Mosquito>().Kill();
                }
                break;

            case Weapons.Rifle:
                RaycastHit2D raycastRifleHit2D = Physics2D.Raycast(transform.position, aimDirection, shotRange, layerMask);
                if (raycastRifleHit2D.collider != null && raycastRifleHit2D.rigidbody.gameObject.CompareTag("Mosquito"))
                {
                    raycastRifleHit2D.rigidbody.gameObject.GetComponent<Mosquito>().Kill();
                }
                break;

            case Weapons.Flamethrower:
                GameObject newFlame = mainPool.GetFlame();
                newFlame.transform.position = new Vector3(transform.position.x + aimDirection.x * 1.2f, transform.position.y + aimDirection.y * 1.2f, projectileHeight);
                newFlame.GetComponent<Flame>().InitAsProjectile(flamesVelocity * aimDirection);
                break;

            case Weapons.GrenadeLauncher:
                GameObject newGrenade = Instantiate(grenadePrefab, new Vector3(transform.position.x, transform.position.y, projectileHeight), Quaternion.identity);
                newGrenade.GetComponent<Grenade>().SetMovement(aimDirection * grenadeVelocity);
                break;

            case Weapons.Sniper:
                List<RaycastHit2D> raycastSniperHits2D = new List<RaycastHit2D>();
                ContactFilter2D contactFilterSniper2D = new ContactFilter2D();
                Physics2D.Raycast(transform.position, aimDirection, contactFilterSniper2D.NoFilter(), raycastSniperHits2D, shotRange);
                foreach (RaycastHit2D raycastSniperHit2D in raycastSniperHits2D)
                {
                    if (raycastSniperHit2D.rigidbody.gameObject.CompareTag("Mosquito"))
                    {
                        raycastSniperHit2D.rigidbody.gameObject.GetComponent<Mosquito>().Kill();
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Devuelve el cooldown del arma pasada como parámetro
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    private float GetWeaponCooldown(Weapons weapon)
    {
        float weaponCooldown;
        switch (weapon) {
            case Weapons.Pistol:
                weaponCooldown = pistolCooldown;
                break;
            case Weapons.Rifle:
                weaponCooldown = rifleCooldown;
                break;
            case Weapons.Flamethrower:
                weaponCooldown = flamethrowerCooldown;
                break;
            case Weapons.GrenadeLauncher:
                weaponCooldown = grenadeLauncherCooldown;
                break;
            case Weapons.Sniper:
                weaponCooldown = sniperCooldown;
                break;
            default:
                weaponCooldown = 0f;
                break;
        }
        return weaponCooldown;
    }

    /// <summary>
    /// Aplica la velocidad al personaje en función del input
    /// </summary>
    private void ApplyForce()
    {
        InputValueMovementKeyboardFix();
        rigidbody.velocity = Vector2.ClampMagnitude(inputValueMovement * movementModifier, movementModifier);
    }

    /// <summary>
    /// Convierte el valor de 0.7 y -0.7 a 1 y -1 respectivamente
    /// </summary>
    private void InputValueMovementKeyboardFix()
    {
        if (Mathf.Abs(inputValueMovement.x) > 0.5)
        {
            inputValueMovement.x = 1 * Mathf.Sign(inputValueMovement.x);
        }
        if (Mathf.Abs(inputValueMovement.y) > 0.5)
        {
            inputValueMovement.y = 1 * Mathf.Sign(inputValueMovement.y);
        }
    }

    /// <summary>
    /// Reproduce una animación dependiendo de la velocidad del personaje y de si está disparando.
    /// Este método está diseñado (junto con la variable weaponChangeRequested) específicamente por las limitaciones de las animaciones del personaje.
    /// Es necesario replantear esta parte en caso de usar otro asset.
    /// </summary>
    private void PlayAnimations()
    {
        if (rigidbody.velocity.magnitude > 0)
        {
            if (!pullingTrigger) 
            {
                if (!isDrawnWeaponAutomatic)
                {
                    if (timeSinceLastShot >= shootAnimationTime)
                    {
                        actions.Walk();
                    }
                } else
                {
                    actions.Walk();
                }
            }
        } else
        {
            actions.Aiming();
        }
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Devuelve un Vector3 que contiene:
    /// - (x, y) la dirección en la que apunta el personaje normalizada
    /// - (z) distancia del punto del ratón del personaje
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAimPoint()
    {
        Vector3 retValue;
        retValue.x = aimDirection.x;
        retValue.y = aimDirection.y;
        retValue.z = aimDistance;
        return retValue;
    }

    /// <summary>
    /// Aplica x puntos de daño al personaje
    /// Reproduce el sonido de daño
    /// Comprueba si el jugador ha muerto
    /// Comunica la nueva vida a la UI
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            uiController.SetHealth(0);
            Die();
        }
        else
        {
            soundController.PlayDamageSound();
            uiController.SetHealth(health);
        }
    }
    #endregion    
}
