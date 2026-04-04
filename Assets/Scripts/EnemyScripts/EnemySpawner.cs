using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour, IHit
{
    public WaveData waveSettings;

    public float health = 100f;

    public float spreadRadius = 2f; //Rayon de repartition

    // Flag pour ne pas relancer l'animation de mort
    private bool _isDead = false;

    
    private List<GameObject> _myActiveEnemies = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterSpawner(this);
        }
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        //Batch initiale
        AudioManager.Instance.PlaySound(waveSettings.soundToPlay);
        yield return StartCoroutine(SpawnBatch(waveSettings.initialBatchSize));
        
        //Batches regulieres
        while (health > 0)
        {
            yield return new WaitForSeconds(waveSettings.timeBetweenWaves);
            //On nettoie les references mortes avant de verifier la limite
            CleanEnemyList();
            
            //On ne lance un nouveau batch que si on n'a pas atteint le quota d'ennemie
            if (_myActiveEnemies.Count < waveSettings.maxActiveEnemies)
            {
                AudioManager.Instance.PlaySound(waveSettings.soundToPlay);
                yield return StartCoroutine(SpawnBatch(waveSettings.regularBatchSize));  
            }
            
            
        }
    }


    IEnumerator SpawnBatch(int batchSize)
    {
        
        for (int i = 0; i < batchSize; i++)
        {
            CleanEnemyList();
            if (_myActiveEnemies.Count < waveSettings.maxActiveEnemies)
            {
                SpawnEnemy();
            }
            else
            {
                break;
            }
            
            yield return new WaitForSeconds(waveSettings.spawnIntervalWithinBatch);
        }
    }

    void SpawnEnemy()
    {
        //Choisir un prefab aleatoire dans la liste
        int index = Random.Range(0, waveSettings.enemyPrefabs.Count);
        GameObject enemyPrefab = waveSettings.enemyPrefabs[index];

        //Spawn a la position du spawner
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        
        
        //On enregistre l'ennemi dans la liste locale du spawner
        _myActiveEnemies.Add(enemy);
        
        //Repartition pour ne pas clump
        Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
        Vector3 targetPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y,0);
        
        //on deplace l'ennemi vers sa position repartie 
        enemy.transform.position = targetPosition;

        EnemyAI ai = enemy.GetComponent<EnemyAI>();

        if (ai != null)
        {
            ai.StateMachine.Initialize(ai.PatrolState);
        } 
        
    }
    //Methode utilitaire pour nettoyer les ennemis detruits
    private void CleanEnemyList()
    {
        _myActiveEnemies.RemoveAll(e=>e == null);
    }
    private void TakeDamage(float damage)
    {
        if (TryGetComponent<FlashEffect>(out var flashEffect)) { flashEffect.Flash(); }

        if (AccessibilityManager.Instance != null)
            damage = AccessibilityManager.Instance.ModifyPlayerDamageDealt(damage);


        if (TryGetComponent<EnemyHealthBar>(out var healthBar)) { healthBar.TakeDamage(damage); }
        health -= damage;
        Bloodstains._instance.SpawnBlood(transform.position, -transform.up);
        if (health <= 0 && !_isDead)
        {
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.UnRegisterSpawner(this);
            }
            AudioManager.Instance.PlaySound(waveSettings.soundDeath);
            GetComponent<Animator>()?.SetTrigger("Die");
            _isDead = true;
        }
    }

    public void OnHit(float damage)
    {
	    TakeDamage(damage);
    }

    public void OnHitRepel(float ff, Vector2 V) {}

    public void OnHitStun(float ff) {}
}
