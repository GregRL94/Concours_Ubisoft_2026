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


        if (dist > enemy.data.attackRange)
        {
            if (enemy.data is KamikazeData kData)
            {
                Explode(kData);
            }else if (Time.time > _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + 1f / enemy.data.fireRate;
            }
        }
        else
        {
            stateMachine.ChangeState(enemy.ChaseState);
        }
    }

    void Explode(KamikazeData kData)
    {
        Debug.Log("BOOOOM");
        if(kData.explosionEffect != null)
            Object.Instantiate(kData.explosionEffect, enemy.transform.position, Quaternion.identity);
        
        //Logique du degats
        Collider2D hit = Physics2D.OverlapCircle(enemy.transform.position, kData.explosionRadius, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            // hit.GetComponent<PlayerHealth>().TakeDamage(kData.explosionDamage);
        }

        // 3. L'ennemi disparaît
        Object.Destroy(enemy.gameObject);
    }
    void Shoot()
    {
        Object.Instantiate(enemy.data.projectilePrefab,enemy.firePoint.position, enemy.firePoint.rotation);
    }
}
