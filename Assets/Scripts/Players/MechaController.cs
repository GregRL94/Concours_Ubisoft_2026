using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

#region Setup Classes
[System.Serializable]
public class LaserShotSetup
{
    [field: SerializeField, Tooltip("Prefab of the laser shot to be instantiated when firing.")]
    public GameObject LaserShotPrefab { get; private set; }

    [field: SerializeField, Range(0f, 50f), Tooltip("Speed of the laser shot.")]
    public float Speed { get; private set; } = 15f;
    [field: SerializeField, Range(0f, 100f), Tooltip("Damage dealt by the laser shot on impact.")]
    public float Damage { get; private set; } = 10f;
    [field: SerializeField, Range(0f, 100f), Tooltip("Time before the laser shot self destructs.")]
    public float Lifetime { get; private set; } = 1f;
    [field: SerializeField, Tooltip("Layers the laser shot can interact with.")]
    public LayerMask LaserImpactLayerMask { get; private set; }
}


[System.Serializable]
public class DispersionSetup
{
    [field: SerializeField, Range(0f, 90f), Tooltip("Maximum absolute angular dispersion that can be applied to the guns (degrees).")]
    public float MaxAngluarDispersion { get; private set; } = 30f;

    [field: SerializeField, Range(0f, 5f), Tooltip("Rate at which dispersion increases with each shot (degrees).")]
    public float AngularDispersionRate { get; private set; } = 0.5f;

    [field: SerializeField, Range(0f, 5f), Tooltip("Speed at which the current dispersion resets back to zero when not shooting (degrees).")]
    public float AngularDispersionResetSpeed { get; private set; } = 2f;

    [field: SerializeField, Range(0f, 0.5f), Tooltip("Maximum linear dispersion, as a percentage of the distance to the target, that can be applied to the guns.")]
    public float MaxLinearDispersion { get; private set; } = 0.1f;

    [field: SerializeField, Range(0f, 0.25f), Tooltip("Rate at which linear dispersion increases with each shot, as a percentage of the distance to the target.")]
    public float LinearDispersionRate { get; private set; } = 0.025f;

    [field: SerializeField, Range(0f, 0.25f), Tooltip("Speed at which the current linear dispersion resets back to zero when not shooting, as a percentage of the distance to the target.")]
    public float LinearDispersionResetSpeed { get; private set; } = 0.05f;
}


[System.Serializable]
public class MissileSetup
{
    [field: SerializeField, Tooltip("Prefab of the missile to be instantiated when firing the ultimate attack.")]
    public GameObject MissilePrefab { get; private set; }

    [field: SerializeField, Range(0f, 50f), Tooltip("Speed of the missile.")]
    public float Speed { get; private set; } = 10f;

    [field: SerializeField, Range(0f, 360f), Tooltip("Rotation speed of the missile in degrees per second.")]
    public float RotationSpeed { get; private set; } = 90f;

    [field: SerializeField, Range(0f, 100f), Tooltip("Damage dealt by the missile on impact.")]
    public float Damage { get; private set; } = 30f;

    [field: SerializeField, Range(0f, 10f), Tooltip("Damage dealt by the missile explosion.")]
    public float AOEDamage { get; private set; } = 15f;

    [field: SerializeField, Range(0f, 2f), Tooltip("Time before the missile can start turning towards its target.")]
    public float HoldRotationTimer { get; private set; } = 0.5f;

    [field: SerializeField, Range(0f, 2f), Tooltip("Time before the missile can start moving towards its target.")]
    public float HoldMovementTimer { get; private set; } = 1.5f;

    [field: SerializeField, Range(0f, 100f), Tooltip("Time before the missile self destructs.")]
    public float Lifetime { get; private set; } = 5f;

    [field: SerializeField, Tooltip("Layers the missile can interact with.")]
    public LayerMask MissileImpactLayerMask { get; private set; }
}
#endregion Setup Classes

#region Main Mecha Controller Class
public class MechaController : MonoBehaviour, IHit
{
    #region Attributes & Properties
    public LaserShotSetup LaserShotParameters;
    public DispersionSetup DispersionParameters;
    public MissileSetup MissileParameters;

    [Header("GENERAL SETTINGS")]
    [SerializeField] private GameObject _mechaBase;
    [SerializeField] private GameObject _mechaTop;
    [SerializeField] private GameObject _slash;
    [SerializeField] private GameObject _boost0;
    [SerializeField] private GameObject _boost1;
    [SerializeField] private float _offsetAngleDeg;
    [Header("Shooting configuration")]
    [SerializeField] private bool _advancedShootingControls = true;
    [SerializeField] private float _mechaTopRotSpeed = 180f;
    [Header("Stun parameters")]
    [SerializeField] private ParticleSystem _stunEffect;
    [SerializeField] private bool _stunStopsAbilities = true;
    [SerializeField] private bool _stunStopsMovement = true;
    [Space]

    [Header("MOVING PLAYER PARAMETERS")]
    [Header("Movement parameters")]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _dashSpeedFactor = 3f;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private bool _dashDoesDamage = true;
    [SerializeField] private LayerMask _dashImpactsWhat;
    [SerializeField] private LayerMask _dashIgnoresWhat;
    [SerializeField] private float _dashDamage = 10f;
    [SerializeField] private Animator animIsPressedDash;
    [SerializeField] private Animator animEffectDash;
    [Header("Melee attack parameters")]
    [SerializeField] private Transform _meleeAttackPoint;
    [SerializeField] private float _meleeAttackRadius = 5f;
    [SerializeField] private LayerMask _meleeAttackImpactsWhat;
    [SerializeField] private bool _meleeAttackStuns = true;
    [SerializeField] private bool _meleeAttackRepels = true;
    [SerializeField] private float _meleeDamage = 10f;
    [SerializeField] private float _meleeAttackStunDuration = 1.5f;
    [SerializeField] private float _meleeAttackRepelForce = 100f;
    [SerializeField] private float _meleeAttackCooldown = 1f;
    [SerializeField] private Animator animIsPressedMelee;
    [SerializeField] private Animator animEffectMelee;
    [Space]

    [Header("SHOOTING PLAYER PARAMETERS")]
    [Header("Guns parameters")]
    [SerializeField] private Transform[] _shootingPoints = new Transform[2];
    [SerializeField] private GameObject _aimingReticle;    
    [SerializeField] private float _aimingReticleSpeed = 3f;
    [SerializeField] private float _aimingReticleMinDistance = 1f;
    [SerializeField] private float _aimingReticleMaxDistance = 10f;
    [SerializeField] private float _aimingReticleScreenMargin = 1.5f;
    [SerializeField] private float _aimingReticleKeepOnScreenSpeed = 5f;
    [SerializeField] private float _aimingReticleKeepOnScreenSmoothTime = 1f;
    [SerializeField] private bool _synchFire = false;
    [SerializeField] private float _fireRate = 2f;
    [SerializeField] private bool _angularDispersion = false;
    [SerializeField] private bool _linearDispersion = false;
    [SerializeField] private Animator animIsPressedGun;
    [SerializeField] private Animator animEffectGun;

    [Header("AOE parameters")]
    [SerializeField] private GameObject _aoeEffectPrefab;
    [SerializeField] private LayerMask _aoeImpactsWhat;
    [SerializeField] private bool _aoeStuns = true;
    [SerializeField] private bool _aoeRepels = true;
    [SerializeField] private float _aoeRadius = 3f;
    [SerializeField] private float _aoeDamage = 5f;
    [SerializeField] private float _aoeStunDuration = 1.5f;
    [SerializeField] private float _aoeRepelForce = 100f;
    [SerializeField] private float _aoeCooldown = 5f;
    [SerializeField] private Animator animIsPressedAOE;
    [SerializeField] private Animator animEffectAOE;
    [Space]

    [Header("ULTIMATE TEAM ATTACK PARAMETERS")]
    [SerializeField] private GameObject _leftMissileLauncher;
    [SerializeField] private GameObject _rightMissileLauncher;
    [SerializeField] private LayerMask _ultimateTargetsWhat;
    [SerializeField] private Transform[] _missilePoints = new Transform[2];
    [SerializeField] private int _maxNumberOfMissiles = 12;
    [SerializeField] private float _minReleaseForce = 1f;
    [SerializeField] private float _maxReleaseForce = 5f;
    [SerializeField] private float _missileSpawnInterval = 0.2f;
    [SerializeField] private Animator animIsPressedMovement;
    [SerializeField] private Animator animIsPressedShot;
    [SerializeField] private float _ultimateHoldDuration = 3f;
    [SerializeField] private float _ultimateMax = 100f;
    [SerializeField] private Animator animIsReadyUltimate;
    [SerializeField] private float _ultimateHoldThreshold = 0.25f;
    private float _ultimateCharge = 0f;
    private bool _ultimateReady = false;
    
    private bool _movementCharged = false;
    private bool _shootCharged = false;

    private float _movementHoldTimer = 0f;
    private float _shootHoldTimer = 0f;

    private bool _movementHoldWasShort = true;
    private bool _shootHoldWasShort = true;

    private bool _movementAttemptingUltimate = false;
    private bool _shootAttemptingUltimate = false;

    private bool _ignoreNextMeleeRelease = false;
    private bool _ignoreNextAOERelease = false;

    private bool meleeHold;
    private bool aoeHold;
    private bool dashHold;
    private bool laserHold;

    private float _inputBlockTimer;

    [Header("Mecha Ability Ref")]
    [SerializeField] private MechaAbilityUI abilityUI;
    [SerializeField] private MechaUltimateUI ultimateUI;
    [SerializeField] private AfterImageEffect afterImageEffect;

    private PlayerInputHandler movementPlayer;
    private PlayerInputHandler shootPlayer;
    private MechaHealth _mechaHealth;
    private Coroutine _currentDashCoroutine;
    private Coroutine _currentUltimateCoroutine;
    private Rigidbody2D _rb2D;
    private BoxCollider2D _bc2D;
    private Animator _animatorMechaBase;
    private Animator _animatorMechaTop;

    private float _currentSpeed;
    private Vector3 _currentReticleVeloocity;
    private float _dashCooldownTimer;
    private bool _isDashing;
    private bool _dashCollisionsDisabled = false;
    private float _dashCollisionDisableDuration = 0f;
    private float _dashCollisionDisableTimer;
    private float _laserCoolDown;
    private float _meleeTimer;
    private bool _meleeHoldMovement = false;
    private float _meleeHoldMovementDuration = 0.5f;
    private float _meleeHoldMovementTimer;
    private float _aoeTimer;
    private Vector2 _lastNonZeroDir = new Vector2(1f, 0f);
    private int _currentGunIndex = 0;
    private float _currentAngularDispersion;
    private float _currentLinearDispersion;
    private bool _isPlayingMvtSound = false;
    private bool _hasPlayedUltimateReadySound = false;
    private bool _isStun;
    private float _stunTimer;
    private bool _canRechargeUltimate = true;
    private float _rechargeUltimateDelay = 5f;
    private float _rechargeUltimateTimer;
    #endregion Attributes & Properties

    #region MonoBehaviour Methods
    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _bc2D = GetComponent<BoxCollider2D>();
        _animatorMechaBase = _mechaBase.GetComponent<Animator>();
        _animatorMechaTop = _mechaTop.GetComponent<Animator>();
        _mechaHealth = GetComponent<MechaHealth>();
        _currentSpeed = _movementSpeed;

        ultimateUI?.Initialize(_ultimateMax);

        Subscribe(true);

        // Désactivation du viseur de précision si les contrôles de tir avancés ne sont pas activés
        if (!_advancedShootingControls) { _aimingReticle.SetActive(false); }
        else { _aimingReticle.transform.SetParent(null, false);}

        // NO COOLDOWN ABILITY ON START
        _aoeTimer = _aoeCooldown;
        _dashCooldownTimer = _dashCooldown;
        _meleeTimer = _meleeAttackCooldown;

    }

    // Mise à jour des timers de cooldowns, gestion des abilités actives et des collisions de dash
    private void Update()
    {
        if (Time.timeScale == 0f) return;

        // Bloque les inputs pour éviter d’activer des capacités en quittant le menu pause
        if (_inputBlockTimer > 0f)
        {
            _inputBlockTimer -= Time.unscaledDeltaTime;
            return;
        }

        UpdateTimers();

        HoldInput();

        if (movementPlayer != null && shootPlayer != null)
            HandleUltimate();
        AnimButtonAbilities();

        if (movementPlayer != null)
            HandleMovement();

        if (shootPlayer != null)
            HandleCombat();

        if (_dashCollisionsDisabled && (_bc2D.excludeLayers & _dashIgnoresWhat) == 0) { _bc2D.excludeLayers |= _dashIgnoresWhat; } // Ignore les collisions avec les layers ignores pendant le dash
        else if (!_dashCollisionsDisabled && (_bc2D.excludeLayers & _dashIgnoresWhat) != 0) { _bc2D.excludeLayers &= ~_dashIgnoresWhat; } // Reactive les collisions avec les layers ignores apres le dash
    }

    private void LateUpdate()
    {
        if (_advancedShootingControls)
        {
            KeepReticleInScreen(_aimingReticleScreenMargin);
        }
    }
    #endregion MonoBehaviour Methods

    #region Input Bindings
    // Injecte les inputs des joueurs selon leur role choisi
    public void GameplayInitialize(PlayerInputHandler p1, PlayerInputHandler p2)
    {
        if (p1.Role == PlayerRole.Movement)
        {
            movementPlayer = p1;
            shootPlayer = p2;
        }
        else
        {
            movementPlayer = p2;
            shootPlayer = p1;
        }
    }

    private void HoldInput()
    {
        // HOLD INPUT
        meleeHold = movementPlayer.MeleeHold();
        aoeHold = shootPlayer.AOEHold();
        dashHold = movementPlayer.DashHold();
        laserHold = shootPlayer.ShootHold();
    }

    public void BlockInputs(float duration = 0.25f)
    {
        _inputBlockTimer = duration;
    }

    private void HandleMovement()
    {
        // --------------- SI STUN STOP ICI ---------------
        if ((_isStun && _stunStopsMovement) || _meleeHoldMovement)
        {
            if (_isDashing)
            {
                _rb2D.linearVelocity = _lastNonZeroDir * _currentSpeed;
                return;
            }
            _animatorMechaBase.SetBool("isMoving", false);
            return;
        }


        Vector2 move = movementPlayer.GetMovement();
        if (move != Vector2.zero)
        {
            _lastNonZeroDir = move;
            _animatorMechaBase.SetBool("isMoving", true);

            // Son de mouvement du mech
            if (!_isPlayingMvtSound && GameObject.Find("SFX_Player_movement") == null)
            {
                AudioManager.Instance.PlaySound("SFX_Player_movement");
                _isPlayingMvtSound = true;
            }
        }
        else
        {
            _animatorMechaBase.SetBool("isMoving", false);

            if (_isPlayingMvtSound)
            {
                AudioManager.Instance.StopSound("SFX_Player_movement", true);
            }
            _isPlayingMvtSound = false;
        }


        //if (movementPlayer.MeleeReleased()
        //    && _movementHoldWasShort
        //    && !_movementAttemptingUltimate
        //    /*&& _meleeTimer >= _meleeAttackCooldown*/)
        //{
        //    if (_meleeTimer < _meleeAttackCooldown)
        //    {
        //        AudioManager.Instance.PlaySound("SFX_CooldownError");
        //        return;
        //    }

        //    MeleeAttack(move);
        //}

        if (movementPlayer.MeleeReleased())
        {
            if (_ignoreNextMeleeRelease)
            {
                _ignoreNextMeleeRelease = false;
                return;
            }

            if (_movementHoldWasShort && !_movementAttemptingUltimate)
            {
                if (_meleeTimer < _meleeAttackCooldown)
                {
                    abilityUI?.TriggerAbilityCooldownRedFlag("Melee"); // effet rouge
                    AudioManager.Instance.PlaySound("SFX_CooldownError");
                    return;
                }

                MeleeAttack(move);
            }
        }


        if (movementPlayer.DashReleased() /*&& _dashCooldownTimer >= _dashCooldown*/)
        {
            if(_dashCooldownTimer < _dashCooldown)
            {
                abilityUI?.TriggerAbilityCooldownRedFlag("Dash"); 
                AudioManager.Instance.PlaySound("SFX_CooldownError");
                return;
            }

            if (_currentDashCoroutine != null)
            {
                StopCoroutine(_currentDashCoroutine);
            }
            AudioManager.Instance.PlaySound("SFX_Player_dash");
            _currentDashCoroutine = StartCoroutine(Dash());
        }



        if (_isDashing && move == Vector2.zero)
        {
            move = _lastNonZeroDir;
        }
        _rb2D.linearVelocity = move * _currentSpeed; // Deplacement du mecha selon les inputs du joueur de mouvement
        _mechaBase.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(_lastNonZeroDir.x, _lastNonZeroDir.y, _offsetAngleDeg)); // Rotation de la base du mecha selon la direction de d�placement
    }

    private void HandleCombat()
    {
        // --------------- SI STUN STOP ICI ---------------
        if (_isStun && _stunStopsAbilities)
        {
            _animatorMechaTop.SetBool("isShooting", false);
            ResetDispersions();
            return;
        }

        Vector2 aim = shootPlayer.GetAim();

        if (_advancedShootingControls) { AimReticle(aim); }
        else { RotateUpperMech(aim.x); }

        if (shootPlayer.ShootPressed() && _laserCoolDown >= 1 / _fireRate)
        {
            ShootLaser(LaserShotParameters.LaserShotPrefab, _synchFire, _advancedShootingControls);
        }
        else
        {
            ResetDispersions();
        }

        //if (shootPlayer.AOEReleased()
        //    && _shootHoldWasShort
        //    && !_shootAttemptingUltimate
        //    /*&& _aoeTimer >= _aoeCooldown*/)
        //{
        //    if (_aoeTimer < _aoeCooldown)
        //    {
        //        AudioManager.Instance.PlaySound("SFX_CooldownError");
        //        return;
        //    }

        //    Shockwave(_aoeRadius, _aoeDamage, _aoeRepelForce);
        //}

        if (shootPlayer.AOEReleased())
        {
            if (_ignoreNextAOERelease)
            {
                _ignoreNextAOERelease = false;
                return;
            }

            if (_shootHoldWasShort && !_shootAttemptingUltimate)
            {
                if (_aoeTimer < _aoeCooldown)
                {
                    abilityUI?.TriggerAbilityCooldownRedFlag("AOE");
                    AudioManager.Instance.PlaySound("SFX_CooldownError");
                    return;
                }

                Shockwave(_aoeRadius, _aoeDamage, _aoeRepelForce);
            }
        }

        if (shootPlayer.ShootPressed())
        {
            _animatorMechaTop.SetBool("isShooting", true);
        }
        else
        {
            _animatorMechaTop.SetBool("isShooting", false);
        }
    }

    private void HandleUltimate()
    {
        // UPDATE ULTIMATE UI FILL
        ultimateUI?.UpdateCharge(_ultimateCharge);

        // SI STUN STOP ICI 
        if (_isStun && _stunStopsAbilities)
            return;

        if (_ultimateCharge >= _ultimateMax)
        {
            _ultimateCharge = _ultimateMax;
            _ultimateReady = true;
            animIsReadyUltimate.SetBool("isReady", _ultimateReady);
            if (!_hasPlayedUltimateReadySound)
            {
                AudioManager.Instance.PlaySound("UI_ulti_playerready");
                _hasPlayedUltimateReadySound = true;
            }
        }

        // SI ULTIMATE PAS READY STOP ICI 
        if (!_ultimateReady) return;

        _movementAttemptingUltimate = _ultimateReady &&
                             meleeHold &&
                             _movementHoldTimer > _ultimateHoldThreshold;

        _shootAttemptingUltimate = _ultimateReady &&
                                  aoeHold &&
                                  _shootHoldTimer > _ultimateHoldThreshold;

        // ULTIMATE ABILITY HOLD DETECTION 
        if (meleeHold)
        {
            _movementHoldTimer += Time.deltaTime;
            animIsPressedMovement.SetBool("isPressed", true);
            _leftMissileLauncher.GetComponent<Animator>().SetBool("isActivated", true);
        }
        else
        {
            _movementHoldWasShort = _movementHoldTimer < _ultimateHoldThreshold;
            _movementHoldTimer = 0f;
            animIsPressedMovement.SetBool("isPressed", false);
            _leftMissileLauncher.GetComponent<Animator>().SetBool("isActivated", false);
        }

        if (aoeHold)
        {
            _shootHoldTimer += Time.deltaTime;
            animIsPressedShot.SetBool("isPressed", true);
            _rightMissileLauncher.GetComponent<Animator>().SetBool("isActivated", true);
        }
        else
        {
            _shootHoldWasShort = _shootHoldTimer < _ultimateHoldThreshold;
            _shootHoldTimer = 0f;
            animIsPressedShot.SetBool("isPressed", false);
            _rightMissileLauncher.GetComponent<Animator>().SetBool("isActivated", false);
        }

        // CHARGED ULTIMATE 
        if (_movementHoldTimer >= _ultimateHoldDuration)
        {
            _movementCharged = true;
        }

        if (_shootHoldTimer >= _ultimateHoldDuration)
        {
            _shootCharged = true;
        }

        // FILL ULTIMATE BAR
        float syncP1 = 0f;
        float syncP2 = 0f;
        syncP1 += Mathf.Clamp01(_movementHoldTimer / _ultimateHoldDuration);
        syncP2 += Mathf.Clamp01(_shootHoldTimer / _ultimateHoldDuration);

        ultimateUI?.UpdateCoopHoldP1(syncP1);
        ultimateUI?.UpdateCoopHoldP2(syncP2);


        // ACTIVATE ULTIMATE
        Collider2D[] targets;
        bool targetsInArea = CheckAreaForTargets(transform.position, ScreenSizeToWorldBox(), 0f, _ultimateTargetsWhat, out targets);
        if (_movementCharged && _shootCharged && meleeHold && aoeHold && targetsInArea)
            ActivateUltimate(targets);
    }

    private void ActivateUltimate(Collider2D[] targets)
    {
        if (_ultimateReady)
        {
            if (_currentUltimateCoroutine != null) { StopCoroutine(_currentUltimateCoroutine); }
            _currentUltimateCoroutine = StartCoroutine(MissileSwarm(targets));
        }        

        _ultimateReady = false;
        _hasPlayedUltimateReadySound = false;
        animIsReadyUltimate.SetBool("isReady", _ultimateReady);
        _ultimateCharge = 0;

        _movementCharged = false;
        _shootCharged = false;

        _movementHoldWasShort = true;
        _shootHoldWasShort = true;

        _movementAttemptingUltimate = false;
        _shootAttemptingUltimate = false;

        //_movementHoldTimer = 0f;
        //_shootHoldTimer = 0f;

        // evite d'activer les abilities normal apres avoir realiser ultimate attack
        _ignoreNextMeleeRelease = true;
        _ignoreNextAOERelease = true;

        ultimateUI?.UpdateCoopHoldP1(0f);
        ultimateUI?.UpdateCoopHoldP2(0f);
        ultimateUI?.ResetUltimate();
    }

    private void AnimButtonAbilities()
    {
        // ABILITIES UI ANIM SYNC
        if (meleeHold)
        {
            animIsPressedMelee.SetBool("isPressed", true);
        }
        else
        {
            animIsPressedMelee.SetBool("isPressed", false);
        }

        if (aoeHold)
        {
            animIsPressedAOE.SetBool("isPressed", true);
        }
        else
        {
            animIsPressedAOE.SetBool("isPressed", false);
        }

        if (laserHold)
            animIsPressedGun.SetBool("isPressed", true);
        else
            animIsPressedGun.SetBool("isPressed", false);

        if (dashHold)
            animIsPressedDash.SetBool("isPressed", true);
        else
            animIsPressedDash.SetBool("isPressed", false);
    }
    #endregion Input Bindings

    #region Abilities Logic
    private void MeleeAttack(Vector2 attackDir)
    {
        animEffectMelee.SetTrigger("interact");

        // Séparé de la logique propre de la melee attack, permet de hold le mouvement pendant une courte durée après le lancement de l'attaque
        _meleeHoldMovementTimer = _meleeHoldMovementDuration;
        _meleeHoldMovement = true;

        _slash.GetComponent<Animator>().SetTrigger("Slash");
        foreach (Collider2D hitObject in Physics2D.OverlapCircleAll(_meleeAttackPoint.position, _meleeAttackRadius, _meleeAttackImpactsWhat))
        {
            if (hitObject.TryGetComponent(out IHit hitComponent))
            {
                Vector2 repelDirection = (hitObject.transform.position - transform.position).normalized;
                hitComponent.OnHit(ApplyAccessibilityDamageModifier(_meleeDamage));
                if (_meleeAttackStuns) { hitComponent.OnHitStun(_meleeAttackStunDuration); }
                if (_meleeAttackRepels) { hitComponent.OnHitRepel(_meleeAttackRepelForce, repelDirection); }                
            }
        }
        _meleeTimer = 0f;
        AudioManager.Instance.PlaySound("SFX_Player_melee");
        abilityUI?.TriggerAbility("Melee", _meleeAttackCooldown);
    }

    IEnumerator Dash()
    {
        animEffectDash.SetTrigger("interact");
        float startTime = Time.time;
        _isDashing = true;
        _boost0.SetActive(true);
        _boost1.SetActive(true);

        // AfterImageEffect
        afterImageEffect?.PlayDashEffect(_dashDuration);

        // Génère une impulsion de caméra au début du dash si un CinemachineImpulseSource est attaché à la base du mecha
        if (_mechaBase.TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulse();
        }

        // Séparés de la logique propre du dash pour permettre de désactiver les collisions pendant une courte durée après la fin du dash
        _dashCollisionDisableTimer = _dashDuration + _dashCollisionDisableDuration;
        _dashCollisionsDisabled = true; // Ignore les collisions de dash pendant la durée du dash + la durée de désactivation des collisions

        _currentSpeed = _movementSpeed * _dashSpeedFactor;
        while (Time.time < startTime + _dashDuration)
        {
            yield return null;
        }
        _currentSpeed = _movementSpeed;
        _dashCooldownTimer = 0f;
        _boost0.SetActive(false);
        _boost1.SetActive(false);
        _isDashing = false;
        abilityUI?.TriggerAbility("Dash", _dashCooldown);
        yield break;
    }

    private void ShootLaser(GameObject _laserShotPrefab, bool synchFire = false, bool _advancedShooting = true)
    {
        animEffectGun.SetTrigger("interact");
        if (!synchFire)
        {
            if (_currentGunIndex > _shootingPoints.Length - 1) { _currentGunIndex = 0; }
            if (_advancedShooting) { ApplyAngularDispersion(_currentGunIndex); }
            InstantiateShotAtGunIndex(_currentGunIndex, _advancedShooting);
            _currentGunIndex++;
        }
        else
        {
            for (int i = 0; i < _shootingPoints.Length; i++)
            {
                if (_advancedShooting) { ApplyAngularDispersion(i); }
                InstantiateShotAtGunIndex(i, _advancedShooting);
            }
        }
        _laserCoolDown = 0f;
        UpdateDispersions();
        abilityUI?.TriggerAbility("Laser", 1f / _fireRate);
        AudioManager.Instance.PlaySound("SFX_Player_laser_tir");
    }

    private void Shockwave(float radius, float damage, float repelForce)
    {
        animEffectAOE.SetTrigger("interact");
        AudioManager.Instance.PlaySound("SFX_Player_aoe");

        // Instancie l'effet visuel de l'attaque AOE
        Instantiate(_aoeEffectPrefab, transform.position, Quaternion.identity);

        // Génère une impulsion de caméra au début du Shockwave si un CinemachineImpulseSource est attaché à la base du mecha
        if (_mechaTop.TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulse();
        }

        foreach (Collider2D hitObject in Physics2D.OverlapCircleAll(transform.position, radius, _aoeImpactsWhat))
        {
            if (hitObject.TryGetComponent(out IHit hitComponent))
            {
                Vector2 repelDirection = (hitObject.transform.position - transform.position).normalized;
                hitComponent.OnHit(ApplyAccessibilityDamageModifier(damage));
                if (_aoeRepels) { hitComponent.OnHitRepel(repelForce, repelDirection); }
                if (_aoeStuns) { hitComponent.OnHitStun(_aoeStunDuration); }
            }
        }
        _aoeTimer = 0f;
        abilityUI?.TriggerAbility("AOE", _aoeCooldown);
    }

    IEnumerator MissileSwarm(Collider2D[] targets)
    {
        // Séparé de la logique propre de l'ultimate pour permettre de mettre un délai avant de pouvoir recharger l'ultimate après son utilisation
        _rechargeUltimateTimer = _rechargeUltimateDelay;
        _canRechargeUltimate = false;

        int currentLauncherIndex = 0;
        int missilesSpawned = 0;
        
        while (missilesSpawned < _maxNumberOfMissiles)
        {
            int randIndex = Random.Range(0, targets.Length);
            GameObject target = null;
            if (targets[randIndex] != null) { target = targets[randIndex].gameObject; }

            if (currentLauncherIndex > _missilePoints.Length - 1) { currentLauncherIndex = 0; }
            Vector2 ejectionForceDir = (_missilePoints[currentLauncherIndex].position - transform.position).normalized;

            GameObject spawnedMissile = Instantiate(MissileParameters.MissilePrefab, _missilePoints[currentLauncherIndex].position, _missilePoints[currentLauncherIndex].rotation);
            spawnedMissile.GetComponent<Rigidbody2D>().AddForce(ejectionForceDir * Random.Range(_minReleaseForce, _maxReleaseForce), ForceMode2D.Impulse);
            Missile missileLogic = spawnedMissile.GetComponent<Missile>();
            missileLogic.SetupMissile(MissileParameters.Speed,
                MissileParameters.RotationSpeed,
                MissileParameters.Lifetime,
                ApplyAccessibilityDamageModifier(MissileParameters.Damage),
                ApplyAccessibilityDamageModifier(MissileParameters.AOEDamage),
                MissileParameters.HoldRotationTimer,
                MissileParameters.HoldMovementTimer,
                MissileParameters.MissileImpactLayerMask);
            missileLogic.SetTarget(target);

            AudioManager.Instance.PlaySound("SFX_ulti_missile_popout");

            missilesSpawned++;
            currentLauncherIndex++;

            yield return new WaitForSeconds(_missileSpawnInterval);
        }
        _leftMissileLauncher.GetComponent<Animator>().SetBool("isActivated", false);
        _rightMissileLauncher.GetComponent<Animator>().SetBool("isActivated", false);
        yield break;
    }

    private void IncreaseUltimateCharge(float amount)
    {
        if (_canRechargeUltimate) { _ultimateCharge += amount; }
    }

    private void UpdateTimers()
    {
        _dashCooldownTimer += Time.deltaTime;
        _laserCoolDown += Time.deltaTime;
        _meleeTimer += Time.deltaTime;
        _aoeTimer += Time.deltaTime;

        //_ultimateCharge += 1;
        // Movement hold during melee attack timer
        if (_meleeHoldMovementTimer > 0f) { _meleeHoldMovementTimer -= Time.deltaTime; }
        else if (_meleeHoldMovement) { _meleeHoldMovement = false; }


        // Dash collision disable timer
        if (_dashCollisionDisableTimer > 0f) { _dashCollisionDisableTimer -= Time.deltaTime; }
        else if (_dashCollisionsDisabled) { _dashCollisionsDisabled = false; } // Reactive les collisions de dash apres la durée de désactivation

        // Ultimate recharge delay timer
        if (_rechargeUltimateTimer > 0f) { _rechargeUltimateTimer -= Time.deltaTime; }
        else if (!_canRechargeUltimate) { _canRechargeUltimate = true; }

        // Stun timer
        if (_isStun)
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0f)
            {
                _isStun = false;
                _stunTimer = 0f;
                _stunEffect?.Stop();
                _stunEffect?.Clear();
            }
        }
    }
    #endregion Abilities Logic

    #region Damage & Health Logic
    private float ApplyAccessibilityDamageModifier(float damage)
    {
        if (AccessibilityManager.Instance != null)
        {
            damage = AccessibilityManager.Instance.ModifyPlayerDamageDealt(damage);
        }
        return damage;
    }

    private void Stun(float stunDuration)
    {
        if (stunDuration <= 0f) return;
        _isStun = true;
        _stunTimer = stunDuration;
        _stunEffect.Play();
    }

    private void TakeDamage(float damage)
    {
        _mechaHealth.TakeDamage(damage);
    }
    #endregion Damage & Health Logic

    #region Aiming & Shooting Logic
    
    public void SetAdvancedShootingControls(bool enabled)
    {
        if (enabled)
        {
            _aimingReticle.transform.position = _mechaTop.transform.position + _mechaTop.transform.up * _aimingReticleMaxDistance;
            _aimingReticle.SetActive(true);
            _aimingReticle.transform.SetParent(null, false);
            _advancedShootingControls = true;
        }
        else
        {
            _aimingReticle.transform.SetParent(gameObject.transform);
            _shootingPoints[0].transform.eulerAngles = _mechaTop.transform.eulerAngles;
            _shootingPoints[1].transform.eulerAngles = _mechaTop.transform.eulerAngles;
            _aimingReticle.SetActive(false);
            _advancedShootingControls = false;
        }        
    }

    private void RotateUpperMech(float rotDir)
    {
        _mechaTop.transform.Rotate(0f, 0f, -rotDir * _mechaTopRotSpeed * Time.deltaTime);
    }

    private void AimReticle(Vector2 aim)
    {
        float[] boundaries = ScreenBoundaries();
        _aimingReticle.transform.Translate(_aimingReticleSpeed * Time.deltaTime * aim);
        
        // Clamp la position du viseur aux limites de l'ecran
        _aimingReticle.transform.position = new Vector3(
            Mathf.Clamp(_aimingReticle.transform.position.x, boundaries[0], boundaries[2]),
            Mathf.Clamp(_aimingReticle.transform.position.y, boundaries[1], boundaries[3]),
            _aimingReticle.transform.position.z);

        Vector2 aimDirection = _aimingReticle.transform.position - transform.position;
        if (aimDirection != Vector2.zero)
        {
            if (Vector2.Distance(_aimingReticle.transform.position, transform.position) < _aimingReticleMinDistance)
            {
                _aimingReticle.transform.position = (Vector2)transform.position + aimDirection.normalized * _aimingReticleMinDistance; // Empêche le viseur de se rapprocher trop du mecha.
            }

            if (Vector2.Distance(_aimingReticle.transform.position, transform.position) > _aimingReticleMaxDistance)
            {
                _aimingReticle.transform.position = (Vector2)transform.position + aimDirection.normalized * _aimingReticleMaxDistance; // Empêche le viseur de s'éloigner trop du mecha.
            }
            AimGuns(aimDirection);
        }
    }

    private void KeepReticleInScreen(float margin)
    {
        float[] screenBoundaries = ScreenBoundaries();

        float leftBoundary = screenBoundaries[0] + margin;
        float rightBoundary = screenBoundaries[2] - margin;
        float bottomBoundary = screenBoundaries[1] + margin;
        float topBoundary = screenBoundaries[3] - margin;

        Vector3 reticlePos = _aimingReticle.transform.position;
        Vector3 mechDir = transform.position - reticlePos;

        if (reticlePos.x > rightBoundary || reticlePos.x < leftBoundary || reticlePos.y > topBoundary || reticlePos.y < bottomBoundary)
        {
            Vector3 adjustedPos = reticlePos + 1.4f * margin * mechDir.normalized;
            _aimingReticle.transform.position = Vector3.SmoothDamp(reticlePos, adjustedPos, ref _currentReticleVeloocity, _aimingReticleKeepOnScreenSmoothTime, _aimingReticleKeepOnScreenSpeed);
        }
    }

    private void AimGuns(Vector2 aimDirection)
    {
        Vector2 gun0AimDir, gun1AimDir;
        _mechaTop.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aimDirection.x, aimDirection.y, _offsetAngleDeg)); // Rotation du buste du mecha selon la direction de visee
        gun0AimDir = _aimingReticle.transform.position - _shootingPoints[0].position;
        gun1AimDir = _aimingReticle.transform.position - _shootingPoints[1].position;
        _shootingPoints[0].transform.eulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(gun0AimDir.x, gun0AimDir.y, _offsetAngleDeg)); // Rotation du gun 0 selon la direction de visee
        _shootingPoints[1].transform.eulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(gun1AimDir.x, gun1AimDir.y, _offsetAngleDeg)); // Rotation du gun 1 selon la direction de visee
    }

    private float[] ScreenBoundaries()
    {
        Vector2 screenBottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - 1, Screen.height - 1));
        return new float[4] { screenBottomLeft.x, screenBottomLeft.y, screenTopRight.x, screenTopRight.y }; 
    }

    private Vector2 ScreenSizeToWorldBox()
    {
        float[] boundaries = ScreenBoundaries();
        return new Vector2(boundaries[2] - boundaries[0], boundaries[3] - boundaries[1]);
    }

    private void InstantiateShotAtGunIndex(int gunIndex, bool advancedShooting)
    {
        GameObject laserShotGO = Instantiate(LaserShotParameters.LaserShotPrefab, _shootingPoints[gunIndex].position, _shootingPoints[gunIndex].rotation);
        if (laserShotGO.TryGetComponent(out LaserShot laserShotComponent))
        {
            float lifeTime = advancedShooting ? CalculateShotLifeTime(gunIndex) : LaserShotParameters.Lifetime;
            laserShotComponent.SetupLaserShoot(LaserShotParameters.Speed, ApplyAccessibilityDamageModifier(LaserShotParameters.Damage), lifeTime, LaserShotParameters.LaserImpactLayerMask);
        }
    }

    private float CalculateShotLifeTime(int gundIndex)
    {
        float distanceToAimPoint = Vector2.Distance(_shootingPoints[gundIndex].position, _aimingReticle.transform.position);
        if (_linearDispersion) { return ApplyLinearDispersion(distanceToAimPoint) / LaserShotParameters.Speed; }
        return distanceToAimPoint / LaserShotParameters.Speed;
    }

    private bool CheckAreaForTargets(Vector2 center, Vector2 size, float angle, LayerMask layerMask, out Collider2D[] hits)
    {
        hits = Physics2D.OverlapBoxAll(center, size, angle, layerMask);
        return hits.Length > 0;
    }
    #endregion Aiming & Shooting Logic

    #region Dispersion Logic
    private void ApplyAngularDispersion(int gunIndex)
    {
        if (_angularDispersion && _currentAngularDispersion > 0f)
        {
            float randomAngle = Random.Range(-_currentAngularDispersion, _currentAngularDispersion);
            _shootingPoints[gunIndex].transform.Rotate(0f, 0f, randomAngle);
        }
    }

    private float ApplyLinearDispersion(float distance)
    {
        float randDispersion = Random.Range(-_currentLinearDispersion * distance, _currentLinearDispersion * distance);
        return distance + randDispersion;
    }

    private void UpdateDispersions()
    {
        if (_angularDispersion)
        {
            _currentAngularDispersion += DispersionParameters.AngularDispersionRate;
            _currentAngularDispersion = Mathf.Clamp(_currentAngularDispersion, -DispersionParameters.MaxAngluarDispersion, DispersionParameters.MaxAngluarDispersion);
        }

        if (_linearDispersion)
        {
            _currentLinearDispersion += DispersionParameters.LinearDispersionRate;
            _currentLinearDispersion = Mathf.Clamp(_currentLinearDispersion, -DispersionParameters.MaxLinearDispersion, DispersionParameters.MaxLinearDispersion);
        }
    }

    private void ResetDispersions()
    {
        if (_angularDispersion)
        {
            _currentAngularDispersion = Mathf.Lerp(_currentAngularDispersion, 0f, Time.deltaTime * DispersionParameters.AngularDispersionResetSpeed);
        }

        if(_linearDispersion)
        {
            _currentLinearDispersion = Mathf.Lerp(_currentLinearDispersion, 0f, Time.deltaTime * DispersionParameters.LinearDispersionResetSpeed);
        }
    }
    #endregion Dispersion Logic

    #region Collision Logic
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDashing && _dashDoesDamage && ((1 << collision.gameObject.layer) & _dashImpactsWhat) != 0)
        {
            if (collision.collider.TryGetComponent(out IHit hitComponent))
            {
                hitComponent.OnHit(ApplyAccessibilityDamageModifier(_dashDamage)); // Inflige des degats de dash
            }
        }
    }
    #endregion Collision Logic

    #region Subscriptions
    private void Subscribe(bool state)
    {
        if (state)
        {
            GameManager.OnUltimateJaugeIncrease += IncreaseUltimateCharge;
            AccessibilityMenu.OnSetAdvancedShooting += SetAdvancedShootingControls;
            SetAdvancedShootingControls(AccessibilityManager.Instance.GetAimMode()); 

        }
        else
        {
            GameManager.OnUltimateJaugeIncrease -= IncreaseUltimateCharge;
            AccessibilityMenu.OnSetAdvancedShooting -= SetAdvancedShootingControls;
        }
    }

    private void OnDisable()
    {
        Subscribe(false);
    }

    private void OnDestroy()
    {
        Subscribe(false);
    }
    #endregion Subscribtions

    #region IHit Implementation
    public void OnHit(float damage)
    {
        TakeDamage(damage);
    }

    public void OnHitRepel(float repelForce, Vector2 repelDirection)
    {
        _rb2D.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);
    }

    public void OnHitStun(float stunDuration)
    {
        Stun(stunDuration);
    }
    #endregion IHit Implementation
}
#endregion Main Mecha Controller Class
