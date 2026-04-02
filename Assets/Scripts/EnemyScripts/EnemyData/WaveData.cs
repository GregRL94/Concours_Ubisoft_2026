using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Wave System/WaveData")]
public class WaveData : ScriptableObject
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public int initialBatchSize = 10;
    public int regularBatchSize = 5;
    public float timeBetweenWaves = 5f;
    public float spawnIntervalWithinBatch = 0.2f;
    public int maxActiveEnemies;
    public string soundToPlay;
}
