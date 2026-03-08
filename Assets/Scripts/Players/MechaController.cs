using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MechaController : MonoBehaviour
{
    [Header("GENERAL SETTINGS")]
    [SerializeField] private GameObject _mechaBase;
    [SerializeField] private GameObject _mechaTop;
    [SerializeField] private float _offsetAngleDeg;
    [Space]

    [Header("MOVING PLAYER PARAMETERS")]
    [Header("Movement parameters")]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _dashSpeedFactor = 3f;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private bool _dashDoesDamage = true;
    [SerializeField] private LayerMask _dashImpactsWhat;
    [SerializeField] private float _dashDamage = 10f;
    [Header("Melee attack parameters")]
    [SerializeField] private Transform _meleeAttackPoint;
    [SerializeField] private Vector2 _meleeAttackHitBox;
    [SerializeField] private LayerMask _meleeAttackImpactsWhat;
    [SerializeField] private float _meleeDamage = 10f;
    [SerializeField] private float _meleeAttackCooldown = 1f;
    [Space]

    [Header("SHOOTING PLAYER PARAMETERS")]
    [Header("Shooting parameters")]
    [SerializeField] private GameObject _aimingCursor;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private GameObject _laserShotPrefab;
    [SerializeField] private LayerMask _laserImpactsWhat;
    [SerializeField] private float _aimingCursorSpeed = 3f;
    [SerializeField] private float _fireRate = 2f;
    [SerializeField] private float _laserShotSpeed;
    [SerializeField] private float _laserShotDamage = 5f;
    [SerializeField] private float _laserShotLifeTime = 10f;

    [Header("AOE parameters")]
    [SerializeField] private LayerMask _aoeImpactsWhat;
    [SerializeField] private float _aoeRadius = 3f;
    [SerializeField] private float _aoeDamage = 5f;
    [SerializeField] private float _aoeRepelForce = 2f;
    [SerializeField] private float _aoeCooldown = 5f;

    [Header("Ultimate Team Attack parameters")]
    [SerializeField] private float _ultimateHoldDuration = 3f;
    [SerializeField] private float _ultimateMax = 100f;
    [SerializeField] private float _ultimateInputForgiveness = 0.12f;
    private float _ultimateCharge = 0f;
    private bool _isAttemptingUltimate = false;
    private bool _ultimateReady = true; // debug pour l'instant
    private float _ultimateHoldTimer = 0f;

    [Header("Mecha Ability Ref")]
    [SerializeField] private MechaAbilityUI abilityUI;
    [SerializeField] private MechaUltimateUI ultimateUI;

    private PlayerInputHandler movementPlayer;
    private PlayerInputHandler shootPlayer;
    private Coroutine _currentDashCoroutine;
    private Rigidbody2D _rb2D;

    private float _currentSpeed;
    private float _dashCooldownTimer;
    private bool _isDashing;
    private float _laserCoolDown;
    private float _meleeTimer;
    private float _aoeTimer;
    private Vector2 _lastNonZeroDir;

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
    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _currentSpeed = _movementSpeed;

        ultimateUI?.Initialize(_ultimateMax);

    }

    // Mettre a jour la fonctionnalit� des joueurs
    private void Update()
    {
        UpdateTimers();

        if (movementPlayer != null && shootPlayer != null)
            HandleUltimate();

        // si tentative d'ultimate -> on bloque les abilities
        if (_isAttemptingUltimate)
            return;

        if (movementPlayer != null)
            HandleMovement();

        if (shootPlayer != null)
            HandleCombat();
    }

    private void HandleMovement()
    {
        Vector2 move = movementPlayer.GetMovement();
        if (move != Vector2.zero)
        {
            _lastNonZeroDir = move;
        }

        if (movementPlayer.MeleePressed() && _meleeTimer >= _meleeAttackCooldown)
        {
            MeleeAttack(move, _meleeAttackHitBox.x, _meleeAttackHitBox.y, _meleeAttackImpactsWhat);
        }

        if (movementPlayer.GrapplePressed() && _dashCooldownTimer >= _dashCooldown)
        {
            if (_currentDashCoroutine != null)
            {
                StopCoroutine(_currentDashCoroutine);
            }
            _currentDashCoroutine = StartCoroutine(Dash());
            if (_mechaBase.TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
            {
                impulseSource.GenerateImpulse();
            }
        }

        _rb2D.linearVelocity = move * _currentSpeed; // D�placement du mecha selon les inputs du joueur de mouvement
        _mechaBase.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(_lastNonZeroDir.x, _lastNonZeroDir.y, _offsetAngleDeg)); // Rotation de la base du mecha selon la direction de d�placement
    }

    private void HandleCombat()
    {
        Vector2 aim = shootPlayer.GetAim();

        _aimingCursor.transform.Translate(_aimingCursorSpeed * Time.deltaTime * aim);
        
        Vector2 aimDirection = _aimingCursor.transform.position - transform.position;
        if (aimDirection != Vector2.zero)
        {
            _mechaTop.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aimDirection.x, aimDirection.y, _offsetAngleDeg)); // Rotation du buste du mecha selon la direction de vis�e
        }
        //if (aim != Vector2.zero)
        //{
        //    _mechaTop.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aim.x, aim.y, _offsetAngleDeg)); // Rotation du buste du mecha selon la direction de vis�e
        //}

        if (shootPlayer.ShootPressed() && _laserCoolDown >= 1 / _fireRate)
        {
            ShootLaser(_laserShotPrefab, _shootingPoint.position, _shootingPoint.rotation);
        }

        if (shootPlayer.AOEPressed() && _aoeTimer >= _aoeCooldown)
        {
            GroundSmash(_aoeRadius, _aoeDamage, _aoeRepelForce);
            if (_mechaTop.TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
            {
                impulseSource.GenerateImpulse();
            }
        }
    }

    private void HandleUltimate()
    {
        ultimateUI?.UpdateCharge(_ultimateCharge);

        if (_ultimateCharge >= _ultimateMax)
        {
            _ultimateCharge = _ultimateMax;
            _ultimateReady = true;
        }

        if (!_ultimateReady)
            return;

        bool movementCombo = movementPlayer.UltimateComboPressed();
        bool shootCombo = shootPlayer.UltimateComboPressed();

        _isAttemptingUltimate = movementCombo || shootCombo;

        // les deux joueurs doivent hold
        if (movementCombo && shootCombo)
        {
            _ultimateHoldTimer += Time.deltaTime;

            ultimateUI?.UpdateCoopHold(_ultimateHoldTimer / _ultimateHoldDuration);

            if (_ultimateHoldTimer >= _ultimateHoldDuration)
            {
                ActivateUltimate();
            }
        }
        else
        {
            _ultimateHoldTimer = 0f;
            ultimateUI?.UpdateCoopHold(_ultimateHoldTimer);
        }
    }

    private void ActivateUltimate()
    {
        Debug.Log("ULTIMATE TEAM ATTACK UNLEASHED !!!");

        _ultimateReady = false;
        _ultimateCharge = 0;
        _ultimateHoldTimer = 0f;

        _isAttemptingUltimate = false;

        ultimateUI?.UpdateCoopHold(_ultimateHoldTimer);
        ultimateUI?.ResetUltimate();
    }


    IEnumerator Dash()
    {
        float startTime = Time.time;
        _isDashing = true;
        _currentSpeed = _movementSpeed * _dashSpeedFactor;
        while (Time.time < startTime + _dashDuration)
        {
            yield return null;
        }
        _currentSpeed = _movementSpeed;
        _dashCooldownTimer = 0f;
        _isDashing = false;
        abilityUI?.TriggerAbility("Dash", _dashCooldown);
    }

    private void MeleeAttack(Vector2 attackDir, float attackWidth, float attackHeight, LayerMask layerMask)
    {
        float angleRad = MathUtils.DirToAngleRad(attackWidth, attackHeight, _offsetAngleDeg);
        foreach (Collider2D hitObject in Physics2D.OverlapBoxAll(_meleeAttackPoint.position, new Vector2(attackWidth, attackHeight), angleRad, layerMask))
        {
            if (hitObject.TryGetComponent(out IHit hitComponent))
            {
                hitComponent.OnHit(_meleeDamage);
            }
        }
        _meleeTimer = 0f;
        abilityUI?.TriggerAbility("Melee", _meleeAttackCooldown);
        Debug.Log("MELEE");
    }

    private void ShootLaser(GameObject _laserShotPrefab, Vector2 shootingPoint, Quaternion laserShotRotation)
    {
        GameObject laserShotGO = Instantiate(_laserShotPrefab, shootingPoint, laserShotRotation);
        if (laserShotGO.TryGetComponent(out LaserShot laserShotComponent))
        {
            laserShotComponent.SetupLaserShoot(_laserShotSpeed, _laserShotDamage, _laserShotLifeTime, _laserImpactsWhat);
        }
        _laserCoolDown = 0f;

        abilityUI?.TriggerAbility("Laser", 1f / _fireRate);
        //AudioManager.Instance.PlaySound("SFX_Laser");
        Debug.Log("SHOOT");
    }

    private void GroundSmash(float radius, float damage, float repelForce)
    {
        foreach (Collider2D hitObject in Physics2D.OverlapCircleAll(transform.position, radius, _aoeImpactsWhat))
        {
            if (hitObject.TryGetComponent(out IHit hitComponent))
            {
                Vector2 repelDirection = (hitObject.transform.position - transform.position).normalized;
                hitComponent.OnHit(damage, repelForce, repelDirection);
            }
        }
        _aoeTimer = 0f;
        abilityUI?.TriggerAbility("AOE", _aoeCooldown);
        Debug.Log("AOE");
    }

    private void UpdateTimers()
    {
        _dashCooldownTimer += Time.deltaTime;
        _laserCoolDown += Time.deltaTime;
        _meleeTimer += Time.deltaTime;
        _aoeTimer += Time.deltaTime;

        // utimate team attack cooldown
        _ultimateCharge += Time.deltaTime * 10f;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDashing && _dashDoesDamage && ((1 << collision.gameObject.layer) & _dashImpactsWhat) != 0)
        {
            if (collision.collider.TryGetComponent(out IHit hitComponent))
            {
                hitComponent.OnHit(_dashDamage); // Inflige des d�g�ts de dash
                Debug.Log("DASH HIT");
            }
        }
    }
}