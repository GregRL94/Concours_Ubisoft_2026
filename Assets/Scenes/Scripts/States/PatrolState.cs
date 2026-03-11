using UnityEngine;
using UnityEngine.AI; //Obligatoire pour le NavMesh

public class PatrolState : EnemyState
{
    private NavMeshAgent _agent;
    private float _patrolRadius = 22.7f;
    private float _waitTime = 2f;
    private float _timer;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PatrolState(EnemyAI enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
        _agent = enemy.GetComponent<NavMeshAgent>();
        
        //En 2D, il faut empecher l'agent de tourner sur l'axe X ou Y
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public override void Enter()
    {
        SetNewRandomDestination();
    }

    private void SetNewRandomDestination()
    {
        Vector2 randomDir = Random.insideUnitCircle * _patrolRadius;
        Vector3 targetPos = (Vector2)enemy.transform.position + randomDir;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, _patrolRadius, 1))
        {
            _agent.SetDestination(hit.position);
        }
    }
    public override void Update()
    {
       //1. Detection du joueur (Transition vers Chase) 
       if (Vector2.Distance(enemy.transform.position, enemy.Player.position) < enemy.data.detectionRange)
       {
           stateMachine.ChangeState(enemy.ChaseState);
           return;
       }
       
       //2. Verifier si on est arrive au point
       if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
       {
           _timer += Time.deltaTime;
           if (_timer >= _waitTime)
           {
               SetNewRandomDestination();
               _timer = 0f;
           }
       }
    }
}
