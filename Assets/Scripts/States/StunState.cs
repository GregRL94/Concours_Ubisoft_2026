using UnityEngine;

public class StunState : EnemyState
{
    private float stunTimer;

    private float duration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public StunState(EnemyAI enemy, EnemyStateMachine stateMachine, float duration) : base(enemy, stateMachine)
    {
        this.duration = duration;
    }

    public override void Enter()
    {
        base.Enter();
        stunTimer = duration;
        //Arreter le mouvement du NavMeshAgent ou de la velocity
        if (enemy.Agent != null)
        {
            enemy.stunEffect?.Play();
            enemy.Agent.ResetPath();
            enemy.Agent.velocity = Vector3.zero;
        }
    }

    public override void Update()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            if (enemy.DistanceToPlayer <= enemy.data.detectionRange)
            {
                stateMachine.ChangeState(enemy.ChaseState);
            }
            else
            {
                stateMachine.ChangeState(enemy.PatrolState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        if(enemy.Agent != null)
        { 
            enemy.stunEffect?.Stop();
            enemy.stunEffect?.Clear();
        }
    }
}
