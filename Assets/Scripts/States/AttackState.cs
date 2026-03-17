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
            //-- CAS KAMIKAZE --//
            if (enemy.data is KamikazeData kData)
            {
                if (!_isExploding)
                {
                    _isExploding = true;
                    //enemy.Anim.SetTrigger("StartExplosion");// Declenche l'anim de charge/clignotement
                    _explosionRoutine = enemy.StartCoroutine(ExplosionSequence(kData));
                }
                
            }
            //-- CAS SNIPER --//
            else if (enemy.data is SniperData sData)
            {
                // enemy.Anim.SetTrigger("Shoot"); // Declenche l'animation de tir
                Shoot(sData);
                _nextFireTime = Time.time + 1f / sData.fireRate;
            } 
            //-- CAS CORPS A CORPS
            else if (enemy.data is MeleeData mData)
            { 
                // enemy.Anim.SetTrigger("MeeleAttack")' Declenche l'animation d'attack
                PerformMeleeAttack(mData);
                
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
                Vector2 repelDir = (hit.transform.position - enemy.transform.position).normalized;
            
                // On applique les dégâts via l'interface
                hitComponent.OnHit(enemy.data.damage, 10f, repelDir); 
            }
        }

        // L'ennemi se détruit après l'explosion
        Object.Destroy(enemy.gameObject);
    }
    void Shoot(SniperData sData)
    {
        if (sData.projectilePrefab != null && enemy.firePoint != null)
        {
            Debug.Log(enemy.data.enemyName + " tire une balle !");
            GameObject proj = Object.Instantiate(sData.projectilePrefab, enemy.firePoint.position, enemy.firePoint.rotation);
            
            // Si ton projectile a besoin de connaître sa vitesse dès le départ :
            if (proj.TryGetComponent(out Bullet bullet))
            {
                //On passe les degats et la vitesse du sniper
                bullet.Initialize(sData.damage, sData.projectileSpeed);
                //enemy.animator.SetTrigger("Shoot");
            }
        }
    }

    void PerformMeleeAttack(MeleeData mData)
    {
        Debug.Log(enemy.data.enemyName + "donne un coup !");
        // On detecte si le joueur est dans la zone de frappe (devant l'ennemi)
        // On utilise le fire point comme centre de l'attaque s'il existe, sinon le centre de l'ennemi
        Vector2 attackPoint = enemy.firePoint != null ? (Vector2)enemy.firePoint.position : (Vector2)enemy.transform.position;
        
        Collider2D hit = Physics2D.OverlapCircle(attackPoint, mData.hitRadius, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            if (hit.TryGetComponent<IHit>(out IHit target))
            {
                // On peut meme ajouter un recul 
                Vector2 knockBackDir = (hit.transform.position - enemy.transform.position).normalized;
                target.OnHit(mData.damage, 5f, knockBackDir);
                Debug.Log("Joueur touche par le corps a corps");
            }
        }
        
}

}
