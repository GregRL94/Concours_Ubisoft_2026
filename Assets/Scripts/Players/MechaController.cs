using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MechaController : MonoBehaviour
{
    [Header("GENERAL SETTINGS")]
    [SerializeField] private GameObject _mechaBase;
    [SerializeField] private GameObject _mechaTop;
    [SerializeField] private float _offsetAngleDeg;
    [SerializeField] private LayerMask _enemyLayer;
    [Space]

    [Header("MOVING PLAYER PARAMETERS")]
    [Header("Movement parameters")]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _dashSpeedFactor = 3f;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float _dashCooldown = 2f;
    [Header("Melee attack parameters")]
    [SerializeField] private Transform _meleeAttackPoint;
    [SerializeField] private Vector2 _meleeAttackHitBox;
    [SerializeField] private float _meleeDamage = 10f;
    [SerializeField] private float _meleeAttackCooldown = 1f;
    [Space]

    [Header("SHOOTING PLAYER PARAMETERS")]
    [Header("Shooting parameters")]
    [SerializeField] private GameObject _laserShotPrefab;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private float _fireRate = 2f;
    [SerializeField] private float _laserShotSpeed;
    [SerializeField] private float _laserShotDamage = 5f;
    [SerializeField] private float _laserShotLifeTime = 10f;
    [Header("AOE parameters")]
    [SerializeField] private float _aoeRadius = 3f;
    [SerializeField] private float _aoeDamage = 5f;
    [SerializeField] private float _aoeRepelForce = 2f;
    [SerializeField] private float _aoeCooldown = 5f;

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
    private bool _isPlayingMvtSound = false;

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
    }

    // Mettre a jour la fonctionnalité des joueurs
    private void Update()
    {
        UpdateTimers();
        if (movementPlayer != null) { HandleMovement(); }
        if (shootPlayer != null) { HandleCombat(); }
    }

    private void HandleMovement()
    {
        Vector2 move = movementPlayer.GetMovement();
        if (move != Vector2.zero)
        {
            _lastNonZeroDir = move;

            // Son de mouvement du mech
            if (!_isPlayingMvtSound)
            {
                AudioManager.Instance.PlaySound("SFX_Player_movement");
                _isPlayingMvtSound = true;
            }
        }
        else
        {
            if (_isPlayingMvtSound)
            {
                AudioManager.Instance.StopSound("SFX_Player_movement");
                _isPlayingMvtSound = false;
            }
        }

        if (movementPlayer.MeleePressed() && _meleeTimer >= _meleeAttackCooldown)
        {
            MeleeAttack(move, _meleeAttackHitBox.x, _meleeAttackHitBox.y, _enemyLayer);
        }

        if (movementPlayer.GrapplePressed() && _dashCooldownTimer >= _dashCooldown)
        {
            if (_currentDashCoroutine != null)
            {
                StopCoroutine(_currentDashCoroutine);
            }                
            _currentDashCoroutine = StartCoroutine(Dash());
        }

        _rb2D.linearVelocity = move * _currentSpeed; // Déplacement du mecha selon les inputs du joueur de mouvement
        _mechaBase.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(_lastNonZeroDir.x, _lastNonZeroDir.y, _offsetAngleDeg)); // Rotation de la base du mecha selon la direction de déplacement
    }

    private void HandleCombat()
    {
        Vector2 aim = shootPlayer.GetAim();

        if (aim != Vector2.zero)
        {
            _mechaTop.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aim.x, aim.y, _offsetAngleDeg)); // Rotation du buste du mecha selon la direction de visée
        }            

        if (shootPlayer.ShootPressed() && _laserCoolDown >= 1 / _fireRate)
        {
            ShootLaser(_laserShotPrefab, _shootingPoint.position, _shootingPoint.rotation);
        }            

        if (shootPlayer.AOEPressed() && _aoeTimer >= _aoeCooldown)
        {
            GroundSmash(_aoeRadius, _aoeDamage, _aoeRepelForce);
        }
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
        Debug.Log("MELEE");
    }

    private void ShootLaser(GameObject _laserShotPrefab, Vector2 shootingPoint, Quaternion laserShotRotation)
    {
        AudioManager.Instance.PlaySound("SFX_Player_laser_tir");
        GameObject laserShotGO = Instantiate(_laserShotPrefab, shootingPoint, laserShotRotation);
        if (laserShotGO.TryGetComponent(out LaserShot laserShotComponent))
        {
            laserShotComponent.SetupLaserShoot(_laserShotSpeed, _laserShotDamage, _laserShotLifeTime, _enemyLayer);
        }
        _laserCoolDown = 0f;
        Debug.Log("SHOOT");
    }

    private void GroundSmash(float radius, float damage, float repelForce)
    {
        AudioManager.Instance.PlaySound("SFX_Player_aoe");
        foreach (Collider2D hitObject in Physics2D.OverlapCircleAll(transform.position, radius, _enemyLayer))
        {
            if (hitObject.TryGetComponent(out IHit hitComponent))
            {
                Vector2 repelDirection = (hitObject.transform.position - transform.position).normalized;
                hitComponent.OnHit(damage, repelForce, repelDirection);
            }
        }
        _aoeTimer = 0f;
        Debug.Log("AOE");
    }

    private void UpdateTimers()
    {
        _dashCooldownTimer += Time.deltaTime;
        _laserCoolDown += Time.deltaTime;
        _meleeTimer += Time.deltaTime;
        _aoeTimer += Time.deltaTime;
    }
}