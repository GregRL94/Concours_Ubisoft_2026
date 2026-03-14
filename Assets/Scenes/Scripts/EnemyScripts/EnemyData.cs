using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float moveSpeed = 3f;
    public float maxHealth = 100f;
    public float detectionRange = 7f;
    public float attackRange = 4f;
    public float stopDistance = 3f;
    public float damage = 10f;
}
