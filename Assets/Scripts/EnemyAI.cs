using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyState.EnemyStateMachine StateMachine { get; set; }
    [Header("Parametre de deplacement")] 
    public float moveSpeed = 3f;
    //public Transform[] waypoints; // Points de patrouille
    [Header("Parametre de gestion de vie")]
    public int maxHealth = 100;
    private int currentHealth;
    [Header("Parametre de combat")] 
    public float detectionRange = 7f;
    public float attackRange = 4f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    public Transform Player { get; private set; }
    public Rigidbody2D rb { get; private set; }

    //Instance des etats 
    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        StateMachine = new EnemyState.EnemyStateMachine();
        //Initialisastion des etats concrets 
        PatrolState = new PatrolState(this, StateMachine);
        ChaseState = new ChaseState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        StateMachine.Initialize(PatrolState);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.CurrentState.Update();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
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
        Debug.Log("ENEMY DIEEED!!!");
        Destroy(gameObject);
    }
    //Visualisation des portes dans l'editeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}