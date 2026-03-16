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
       
        // Détection des entités dans le rayon d'explosion
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, kData.explosionRadius);
    
        foreach (var hit in hits)
        {
            // Utilisation de IHit pour infliger des dégâts
            if (hit.TryGetComponent(out IHit hitComponent))
            {
                // Calcul de la direction pour le recul (repel force)
                //Vector2 repelDir = (hit.transform.position - enemy.transform.position).normalized;
            
                // On applique les dégâts via l'interface
                //hitComponent.OnHit(enemy.data.damage, 10f, repelDir);
                hitComponent.OnHit(enemy.data.damage);
            }
        }

        // L'ennemi se détruit après l'explosion
        Object.Destroy(enemy.gameObject);
    }
    void Shoot(SniperData sData)
    {
        Object.Instantiate(sData.projectilePrefab,enemy.firePoint.position, enemy.firePoint.rotation);
    }
}
