using UnityEngine;
using UnityEngine.AI; //Obligatoire pour le NavMesh

public class PatrolState : EnemyState
{
    private NavMeshAgent _agent;
    private float _waitTime = 2f;
    private float _timer;
    private Vector3 _startPos;
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
        base.Enter();
        if(_startPos == Vector3.zero)_startPos = enemy.transform.position;
        // enemy.animator.SetBool("isWalking",true);
        _agent.speed =enemy.data.moveSpeed;
        SetNewRandomDestination();
    }

    private void SetNewRandomDestination()
    {
        Vector2 randomDir = Random.insideUnitCircle * enemy.data.patrolRadius;
        
        Vector3 targetPos = _startPos + new Vector3(randomDir.x, randomDir.y,0);
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, enemy.data.patrolRadius, 1))
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

       if (_agent.velocity.sqrMagnitude > 0.1f)
       {
           RotateTowards(_agent.velocity);
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

    private void RotateTowards(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg-90f;
        enemy.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public override void Exit()
    {
        base.Exit();
        // enemy.animator.SetBool("isWalking",true);
    }
}
