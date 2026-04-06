using System.Collections.Generic;
using UnityEngine;

public class TargetsCompass : MonoBehaviour
{
    [SerializeField] private GameObject _creaturesTargetArrowPrefab;
    [SerializeField] private GameObject _structuresTargetArrowPrefab;
    [SerializeField] private int _enemiesCountThreshold;
    [SerializeField] private int _spawnersCountThreshold;

    private List<TargetArrow> _enemiesTargetArrowsList = new List<TargetArrow>();
    private List<TargetArrow> _spawnersTargetArrowsList = new List<TargetArrow>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < _enemiesCountThreshold; i++) { _enemiesTargetArrowsList.Add(NewTargetArrow("creature")); }
        for (int i = 0; i < _spawnersCountThreshold; i++) { _spawnersTargetArrowsList.Add(NewTargetArrow("structure")); }
    }

    private void HandleEnemyCountChanged(int newCountEnemies, int newCountSpawners)
    {
        //Debug.Log($"Enemy count changed: {newCountEnemies} enemies, {newCountSpawners} spawners");
        if (newCountEnemies <= _enemiesCountThreshold && newCountSpawners < 1)
        {
            for (int i = 0; i < _enemiesTargetArrowsList.Count; i++)
            {
                if (i < newCountEnemies)
                {
                    _enemiesTargetArrowsList[i].Target = EnemyManager.Instance.activeEnnemis[i].gameObject;
                }
                else
                {
                    _enemiesTargetArrowsList[i].Target = null;
                }
            }
        }
        else
        {
            foreach (var targetArrow in _enemiesTargetArrowsList)
            {
                targetArrow.Target = null;
            }
        }

        if (newCountSpawners <= _spawnersCountThreshold)
        {
            for (int i = 0; i < _spawnersTargetArrowsList.Count; i++)
            {
                if (i < newCountSpawners)
                {
                    _spawnersTargetArrowsList[i].Target = EnemyManager.Instance.activeSpawners[i].gameObject;
                }
                else
                {
                    _spawnersTargetArrowsList[i].Target = null;
                }
            }
        }
        else 
        { 
            foreach (var targetArrow in _spawnersTargetArrowsList)
            {
                targetArrow.Target = null;
            }
        }
    }

    private TargetArrow NewTargetArrow(string whatTargetArrow="creature", GameObject target = null)
    {
        GameObject targetArrowType = _creaturesTargetArrowPrefab;

        if (whatTargetArrow == "structure")
        {
            targetArrowType = _structuresTargetArrowPrefab;
        }

        GameObject newTargetArrow = Instantiate(targetArrowType, transform);
        newTargetArrow.GetComponent<TargetArrow>().Target = target;
        return newTargetArrow.GetComponent<TargetArrow>();
    }

    private void Subscribe(bool subscribe)
    {
        if (subscribe)
        {
            EnemyManager.OnCountsChanged += HandleEnemyCountChanged;
        }
        else
        {
            EnemyManager.OnCountsChanged -= HandleEnemyCountChanged;
        }
    }

    private void OnEnable()
    {
        Subscribe(true);
    }

    private void OnDisable()
    {
        Subscribe(false);
    }

    private void OnDestroy()
    {
        Subscribe(false);
    }
}
