using System;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour, IHit
{
    [Header("Donnees de l'ennemi")] 
    public EnemyData data; //Nouvellle fiche de stats ScriptableObj
    public Animator animator {get; private set;}
    [Header("Références de combat")]
    public Transform firePoint;
    public EnemyState.EnemyStateMachine StateMachine { get; set; }
    
    
    //Variables privees synchronisees avec Data
    private float currentHealth;
    public NavMeshAgent Agent { get; set; } 
    public Transform Player { get; private set; }
    public Rigidbody2D rb { get; private set; }

    //Instance des etats 
    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }

    // Flag pour ne pas relancer l'animation de mort
    private bool _isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        // Configuration automatique de NavMesh via la Data
        if (Agent != null && data != null)
        {
            Agent.speed = data.moveSpeed;
            Agent.stoppingDistance = data.stopDistance;
            Agent.updatePosition = true;
            Agent.updateRotation = false;
            Agent.updateUpAxis = false;
        }

        StateMachine = new EnemyState.EnemyStateMachine();
        //Initialisastion des etats concrets 
        PatrolState = new PatrolState(this, StateMachine);
        ChaseState = new ChaseState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (data != null) currentHealth = data.maxHealth;
        StateMachine.Initialize(PatrolState);

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterEnemy(this);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.CurrentState.Update();
    }

    public void TakeDamage(float damage)
    {
        if (TryGetComponent<FlashEffect>(out var flashEffect)) { flashEffect.Flash(); }
        currentHealth -= damage;
        if (currentHealth <= 0 && !_isDead)
        {
            Die();
        }
    }
    private void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
    }

    
    private void Die()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnRegisterEnemy(this);
        }

        if (data is KamikazeData kData)
        {
            AttackState.Explode(kData);
        }
        Debug.Log("ENEMY DIEEED!!!");
        animator.SetTrigger("Die");
        _isDead = true;
    }

    //Visualisation des portes dans l'editeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);
    }

    public void OnHit(float damage)
    {
        TakeDamage(damage);
    }

    public void OnHitRepel(float damage, float repelForce, Vector2 repelDirection)
    {
        if(rb!= null)
            rb.AddForce(repelDirection *repelForce, ForceMode2D.Impulse);
        TakeDamage(damage);
    }

    public void OnHitStun(float damage, float stunDuration)
    {
        // Implémenter la logique de stun ici (par exemple, désactiver les mouvements et les attaques pendant stunDuration)
        TakeDamage(damage);
    }
}