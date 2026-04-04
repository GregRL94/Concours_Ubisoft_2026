using System.Collections.Generic;
using UnityEngine;

public class TargetsCompass : MonoBehaviour
{
    [SerializeField] private GameObject _targetArrowPrefab;
    [SerializeField] private int _numberOfEnnemiesThreshold;
    [SerializeField] private int _numberOfSpawnersThreshold;
    [SerializeField] private float _offsetAngleDeg;

    private Dictionary<GameObject, GameObject> _targetArrowDict;
    private int _enemiesCount;
    private int _spawnersCount;
    private int _numberOftargetArrows;
    private bool _allSpawnerArrowsGenerated;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetArrowDict = new Dictionary<GameObject, GameObject>();
        Subscribe(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {

    }

    private void HandleEnemyCountChanged(int newCountEnemies, int newCountSpawners)
    {
        _enemiesCount = newCountEnemies;
        _spawnersCount = newCountSpawners;
    }

    private void NewTargetArrow(GameObject target)
    {

    }

    private void DeactivateTargetArrow()
    {

    }

    private void PointTargetArrowTowardsTarget(GameObject targetArrow, GameObject target)
    {
        Vector2 targetDir = target.transform.position - transform.position;
        targetArrow.transform.localEulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(targetDir.x, targetDir.y, _offsetAngleDeg));
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
}
