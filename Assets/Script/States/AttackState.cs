using UnityEngine;
/**
 * Diego Felipe Duran Lezama
 * 2026-02-20
 */
public class AttackState : EnemyState
{
    private float _nextFireTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AttackState(EnemyAI enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Update()
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.Player.position);


        if (dist > enemy.attackRange)
        {
            stateMachine.ChangeState(enemy.ChaseState);
        }


        if (Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + 1f / enemy.fireRate;
        }
        
    }

    void Shoot()
    {
        Object.Instantiate(enemy.bulletPrefab,enemy.firePoint.position, enemy.firePoint.rotation);
    }
}
