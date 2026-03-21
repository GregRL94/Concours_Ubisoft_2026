using UnityEngine;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance;

    [Header("Damage Multipliers Options")]
    [Range(1f, 3f)] public float playerDamageMultiplier = 1f;
    [Range(1f, 3f)] public float enemyDamageMultiplier = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //float finalDamage = AccessibilityManager.Instance.ApplyPlayerDamage(baseDamage);
        //enemy.TakeDamage(finalDamage);
    }

    public float ApplyPlayerDamage(float baseDamage)
    {
        return baseDamage * playerDamageMultiplier;
    }

    public float ApplyEnemyDamage(float baseDamage)
    {
        return baseDamage * enemyDamageMultiplier;
    }

    public bool IsDamageModified()
    {
        return playerDamageMultiplier != 1f || enemyDamageMultiplier != 1f;
    }
}