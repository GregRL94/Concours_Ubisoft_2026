using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public WaveData waveSettings;

    public float health = 100f;

    public float spreadRadius = 2f; //Rayon de repartition
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            yield return StartCoroutine(SpawnBatch(waveSettings.regularBatchSize));
            
        }
    }


    IEnumerator SpawnBatch(int batchSize)
    {
        for (int i = 0; i < batchSize; i++)
        {
            SpawnEnemy();
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
    
    //BoltBat ne change pas de direction lorsqu'il commence a shoot l'ennemi. Le boltbat continue de shoot vers la derniere direction du joueur
    //Si le mikpin se fait shoot/attack de melee, il doit exploser et il fait ses degats
}
