using UnityEngine;

public class ChaseState : EnemyState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ChaseState(EnemyAI enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Update()
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.Player.transform.position);

        if (dist <= enemy.attackRange)
        { 
            stateMachine.ChangeState(enemy.AttackState); 
            Debug.Log("JE VAAAAAAAIS ATTTAQUEEEER");
        }
        else if(dist>enemy.detectionRange)
            stateMachine.ChangeState(enemy.PatrolState);
        
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position,enemy.Player.position, enemy.moveSpeed * Time.deltaTime);
        
        //Rotation vers le joueur
        Vector2 direction = (enemy.Player.position - enemy.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        enemy.transform.rotation = Quaternion.Euler(0, 0, angle);
        
    }
}
