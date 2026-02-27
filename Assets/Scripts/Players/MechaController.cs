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
    [SerializeField] private Vector2 _meleeAttackHitBox;
    [SerializeField] private float _meleeDamage = 10f;
    [Space]

    [Header("SHOOTING PLAYER PARAMETERS")]
    [Header("Shooting parameters")]
    [SerializeField] private GameObject _laserShotPrefab;
    [SerializeField] private GameObject _shootingPoint;
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _fireRate = 2f;
    [SerializeField] private float _laserDamage = 5f;
    [Header("AOE parameters")]
    [SerializeField] private float _aoeRadius = 3f;
    [SerializeField] private float _aoeDamage = 5f;
    [SerializeField] private float _aoeRepelForce = 2f;

    private PlayerInputHandler movementPlayer;
    private PlayerInputHandler shootPlayer;
    private Coroutine _currentDashCoroutine;
    private float _currentSpeed;    
    private float _dashCooldownTimer;
    private float _laserCoolDown;
    private bool _isDashing;

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
        _currentSpeed = _movementSpeed;
    }

    // Mettre a jour la fonctionnalité des joueurs
    private void Update()
    {
        _dashCooldownTimer += Time.deltaTime;
        _laserCoolDown += Time.deltaTime;
        if (movementPlayer == null || shootPlayer == null)
            return;

        HandleMovement();
        HandleCombat();
    }

    private void HandleMovement()
    {
        Vector2 move = movementPlayer.GetMovement();        

        if (movementPlayer.MeleePressed())
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

        transform.Translate(move * Time.deltaTime * _currentSpeed); // Déplacement du mecha selon les inputs du joueur de mouvement
        _mechaBase.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(move.x, move.y, _offsetAngleDeg)); // Rotation de la base du mecha selon la direction de déplacement
    }

    private void HandleCombat()
    {
        Vector2 aim = shootPlayer.GetAim();

        if (aim != Vector2.zero)
        {
            transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aim.x, aim.y, _offsetAngleDeg)); // Rotation du buste du mecha selon la direction de visée
        }            

        if (shootPlayer.ShootPressed() && _laserCoolDown >= 1 / _fireRate)
        {
            ShootLaser(aim, _shootingPoint.transform.position, _laserShotPrefab);
            _laserCoolDown = 0f;
        }            

        if (shootPlayer.AOEPressed())
            Debug.Log("AOE");
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
        Vector2 boxCenter = new Vector2(attackWidth/2, attackHeight/2) * attackDir;
        Physics2D.OverlapBoxAll(boxCenter, new Vector2(attackWidth, attackHeight), angleRad, layerMask);
        Debug.Log("MELEE");
    }

    private void ShootLaser(Vector2 shootingDir, Vector2 shootingPoint, GameObject _laserShotPrefab)
    {
        GameObject laserShotGO = Instantiate(_laserShotPrefab, shootingPoint, transform.rotation);
        if (laserShotGO.TryGetComponent<LaserShot>(out LaserShot laserShotComponent))
        {
            laserShotComponent.SetupLaserShoot(10f, _laserDamage, _enemyLayer);
        }
        Debug.Log("SHOOT");
    }
}