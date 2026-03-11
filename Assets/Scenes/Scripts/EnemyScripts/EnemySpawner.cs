using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner  instance { get; private set; }

    [System.Serializable]
    public class EnemyContent
    {
        public string name;
        public GameObject[] enemies;
        public float spawnInterval = 1.5f;
    }

    [Header("Configuration")] 
    public List<EnemyContent> waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;

    [Header("Etat actuel")] 
    public int currentWaveIndex = 0;
    private int _enemiesAlive = 0;
    private bool _isSpawning = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
