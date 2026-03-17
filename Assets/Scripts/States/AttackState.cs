using UnityEngine;
/**
 * Diego Felipe Duran Lezama
 * 2026-02-20
 */
public class AttackState : EnemyState
{
    private float _nextFireTime;

    private bool _isExploding = false;

    private Coroutine _explosionRoutine;//On stock la coroutine pour pouvoir l'arreter
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AttackState(EnemyAI enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Update()
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.Player.position);
    
        Debug.Log(dist);
        if (dist <= enemy.data.attackRange)
        {
            Debug.Log("IM IN");
            if (enemy.data is KamikazeData kData)
            {
                if (!_isExploding)
                {
                    _isExploding = true;
                    
                    _explosionRoutine = enemy.StartCoroutine(ExplosionSequence(kData));
                }
                
            }
            else if (enemy.data is SniperData sData)
            {
                Shoot(sData);
                _nextFireTime = Time.time + 1f / sData.fireRate;
            }
        }
        else
        {
            if (_isExploding)
            {
                CancelExplosion();
            }
            stateMachine.ChangeState(enemy.ChaseState);
        }
    }

    private void CancelExplosion()
    {
        if (_explosionRoutine != null)
        {
            enemy.StopCoroutine(_explosionRoutine);
            _explosionRoutine = null;
        }
        _isExploding = false;
    }
    private System.Collections.IEnumerator ExplosionSequence(KamikazeData kData)
    {
        //Ajouter du feedback ici svp 
        //faire clignoter l'ennemi ou changer sa couleur
        //enemy.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(kData.explosionDelay);
        Explode(kData);
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
    void Shoot(SniperData sData)
    {
        Object.Instantiate(sData.projectilePrefab,enemy.firePoint.position, enemy.firePoint.rotation);
    }
}
