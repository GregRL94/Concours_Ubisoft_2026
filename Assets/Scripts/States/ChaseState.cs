using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyState
{
    private NavMeshAgent _agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ChaseState(EnemyAI enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        //On recupere le component 
        _agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.SetBool("isWalking", true);
        // enemy.animator.SetBool("isRunning",true);
        _agent.speed = enemy.data.moveSpeed;
    }

    public override void Update()
    {
        //Transition vers Attaque ou Patrouille 
        if(enemy.DistanceToPlayer <= enemy.data.attackRange) stateMachine.ChangeState(enemy.AttackState);
        else if (enemy.DistanceToPlayer>enemy.data.detectionRange)stateMachine.ChangeState(enemy.PatrolState);
        
        //On donne la cible au navMesh 
        _agent.SetDestination(enemy.Player.position);
        //MIkPIN trouve le joueur, on se deplace plus vite...
       
        //Dash lorsque le joueur est a porte
        Vector2 direction = enemy.Player.position - enemy.transform.position;
        RotateTowards(direction);
    }

    private void RotateTowards(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        enemy.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
   
    public override void Exit()
    {
        base.Exit();
        // enemy.animator.SetBool("isRunning",true);
        _agent.ResetPath();
    }
}
