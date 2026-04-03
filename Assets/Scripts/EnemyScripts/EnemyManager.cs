using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    //Instance statique pour y acceder via EnemyManager.Instance
    public static EnemyManager Instance { get; private set; }

    public static event Action<int, int> OnCountsChanged;

    [Header("Suivis des ennemis")] 
    public List<EnemyAI> activeEnnemis = new List<EnemyAI>();

    [Header("Suivis des spawners")] 
    public List<EnemySpawner> activeSpawners = new List<EnemySpawner>();

    [Header("Suivis des tutorial triggers")]
    public List<TutorialTrigger> activeTutorialTriggers = new List<TutorialTrigger>();

    private bool _levelCompleted = false;

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
            //Debug.Log($"Ennemi ajoute. Total: {activeEnnemis.Count}");
            NotifyCountsChanged();
        }
    }
    //appelr par l'ennemi quand il meurt
    public void UnRegisterEnemy(EnemyAI enemyAI)
    {
        if (activeEnnemis.Contains(enemyAI))
        {
            activeEnnemis.Remove(enemyAI);
            //Debug.Log($"Ennemi retiré. Restants : {activeEnnemis.Count}");
            //if (activeEnnemis.Count == 0)
            //{
            //    OnAllEnemiesCleared();
            //}
            NotifyCountsChanged();
            OnAllEnemiesCleared();
        }


    }

    //Appele par le spawner si present
    public void RegisterSpawner(EnemySpawner spawner)
    {
        if (!activeSpawners.Contains(spawner))
        {
            activeSpawners.Add(spawner);
            Debug.Log($"Spawner ajouté. Total: {activeSpawners.Count}");
            NotifyCountsChanged();
        }
    }

    //appeler par le spawner quand il est destruit
    public void UnRegisterSpawner(EnemySpawner spawner)
    {
        if (activeSpawners.Contains(spawner))
        {
            activeSpawners.Remove(spawner);
            Debug.Log($"Spawner détruit. Restants: {activeSpawners.Count}");
            NotifyCountsChanged();
            OnAllEnemiesCleared();
        }
    }

    //Appelé par le trigger au Start
    public void RegisterTrigger(TutorialTrigger trigger)
    {
        if (!activeTutorialTriggers.Contains(trigger))
        {
            activeTutorialTriggers.Add(trigger);
            //Debug.Log($"Trigger ajouté. Total: {activeTutorialTriggers.Count}");
        }
    }

    //Appelé quand le trigger est détruit
    public void UnRegisterTrigger(TutorialTrigger trigger)
    {
        if (activeTutorialTriggers.Contains(trigger))
        {
            activeTutorialTriggers.Remove(trigger);
            //Debug.Log($"Trigger détruit. Restants: {activeTutorialTriggers.Count}");
            //OnAllEnemiesCleared();
        }
    }

    // Notify event for enemy + spawner count 
    private void NotifyCountsChanged()
    {
        OnCountsChanged?.Invoke(activeEnnemis.Count, activeSpawners.Count);
    }



    private void OnAllEnemiesCleared()
    {
        if (_levelCompleted) return;

        // enleve les nulls refs
        //activeEnnemis.RemoveAll(e => e == null);
        //activeSpawners.RemoveAll(s => s == null);
        //activeTutorialTriggers.RemoveAll(t => t == null);

        if (activeEnnemis.Count == 0 &&
            activeSpawners.Count == 0 &&
            activeTutorialTriggers.Count == 0)
        {
            _levelCompleted = true;

            Debug.Log("ALL CLEAR -> LEVEL COMPLETE");

            GameManager.Instance.CompleteObjective();
        }
    }


}
