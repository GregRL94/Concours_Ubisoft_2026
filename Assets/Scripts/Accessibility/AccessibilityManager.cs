using UnityEngine;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance;

    [Header("Damage Multipliers")]

    [Tooltip("Damage DEALT by the player to enemies (1 = default, >1 = stronger damage)")]
    [Range(1f, 3f)] public float playerDamageDealtMultiplier = 1f;

    [Tooltip("Damage DEALT by enemies to the player (1 = normal, >1 = stronger damage)")]
    [Range(1f, 3f)] public float enemyDamageDealtMultiplier = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // DAMAGE MODIFIERS

    // Modify damage dealt by the player (to enemies)
    public float ModifyPlayerDamageDealt(float damage)
    {
        float customDamage = damage * playerDamageDealtMultiplier;
        return damage * playerDamageDealtMultiplier;
    }

    // Modify damage dealt by enemies (to player)
    public float ModifyEnemyDamageDealt(float damage)
    {
        float customDamage = damage * enemyDamageDealtMultiplier;
        return customDamage;
    }


    //public bool IsModified =>
    //    playerDamageDealtMultiplier != 1f ||
    //    enemyDamageDealtMultiplier != 1f;
}

//if (AccessibilityManager.Instance.IsPlayerDamageModified())
//{
//    print("Avant: Player Damage " + damage);
//    float customDamage = AccessibilityManager.Instance.ApplyPlayerDamage(damage);
//    print("Apres: Player Damage " + customDamage);
//    damage = customDamage;
//}