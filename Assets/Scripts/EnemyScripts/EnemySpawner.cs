using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour, IHit
{
    public WaveData waveSettings;

    public float health = 100f;

    public float spreadRadius = 2f; //Rayon de repartition
    
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


        health -= damage;
        Bloodstains._instance.SpawnBlood(transform.position, -transform.up);
        Debug.Log("Spawner got hit!");
        if (health <= 0)
        {
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.UnRegisterSpawner(this);
            }
            GetComponent<Animator>()?.SetTrigger("Die");
        }
    }

    public void OnHit(float damage)
    {
	    TakeDamage(damage);
    }

    public void OnHitRepel(float ff, Vector2 V) {}

    public void OnHitStun(float ff) {}
    
    //BoltBat ne change pas de direction lorsqu'il commence a shoot l'ennemi. Le boltbat continue de shoot vers la derniere direction du joueur
   //Cap le nombre d'ennemis que les spawner vont spawn
}
