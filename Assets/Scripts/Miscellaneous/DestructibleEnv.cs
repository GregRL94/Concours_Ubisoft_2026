using UnityEngine;

public class DestructibleEnv : MonoBehaviour, IHit
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private int _thresholdDamagedSound = 15;
    private float _currentHealth;
    private float _damageCounter = 0f;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void TakeDamage(float damage)
    {
        if (TryGetComponent<FlashEffect>(out var flashEffect)) { flashEffect.Flash(); }
        foreach (var childFlashEffect in GetComponentsInChildren<FlashEffect>())
        {
            childFlashEffect.Flash();
        }

        if (AccessibilityManager.Instance != null)
            damage = AccessibilityManager.Instance.ModifyPlayerDamageDealt(damage);

        _currentHealth -= damage;
        _damageCounter += damage;
        if (_currentHealth <= 0)
        {
            _Destroy();
        }
        else if (_damageCounter >= _thresholdDamagedSound)
        {
            AudioManager.Instance.PlaySound("SFX_Env_debris_damage");
            _damageCounter = 0;
        }
    }

    #region IHit Implementation
    public void OnHit(float damage)
    {
        TakeDamage(damage);
    }

    public void OnHitRepel(float repelForce, Vector2 repelDirection) { }
    public void OnHitStun(float stunDuration) { }
    #endregion IHit Implementation

    private void _Destroy()
    {
        // Add destruction effects here (e.g., particle effects, sound, etc.)
        AudioManager.Instance.PlaySound("SFX_Env_debris_destroy");
        Destroy(gameObject);
    }
}
