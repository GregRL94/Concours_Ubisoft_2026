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
        _agent.speed = enemy.data.moveSpeed * 1.5f;
    }

    public override void Update()
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.Player.transform.position);
        
        //Transition vers Attaque ou Patrouille 
        if(dist <= enemy.data.attackRange) stateMachine.ChangeState(enemy.AttackState);
        else if (dist>enemy.data.detectionRange)stateMachine.ChangeState(enemy.PatrolState);
        
        //On donne la cible au navMesh 
        _agent.SetDestination(enemy.Player.position);
        
        //Optionnel et a retravailler 
        LootAtPlayer();


    }

    private void LootAtPlayer()
    {
        Vector2 dir = enemy.Player.position - enemy.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        enemy.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    public override void Exit()
    {
        _agent.ResetPath();
    }
}
