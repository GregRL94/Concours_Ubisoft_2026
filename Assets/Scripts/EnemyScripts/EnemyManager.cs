using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    //Instance statique pour y acceder via EnemyManager.Instance
    public static EnemyManager Instance { get; private set; }

    [Header("Suivis des ennemis")] 
    public List<EnemyAI> activeEnnemis = new List<EnemyAI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    //Appele par l'ennemi quand il apparait
    public void RegisterEnemy(EnemyAI enemyAI)
    {
        if (!activeEnnemis.Contains(enemyAI))
        {
            activeEnnemis.Add(enemyAI);
            Debug.Log($"Ennemi ajoute. Total: {activeEnnemis.Count}");
        }
    }
    //appelr par l'ennemi quand il meurt
    public void UnRegisterEnemy(EnemyAI enemyAI)
    {
        if (activeEnnemis.Contains(enemyAI))
        {
            activeEnnemis.Remove(enemyAI);
            Debug.Log($"Ennemi retiré. Restants : {activeEnnemis.Count}");

            if (activeEnnemis.Count == 0)
            {
                OnAllEnemiesCleared();
            }
        }
    }

    private void OnAllEnemiesCleared()
    {
        Debug.Log("All enemies cleared");
        //On peut appeler la gestion du niveau
    }
}
