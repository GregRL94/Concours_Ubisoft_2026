using UnityEngine;

public class DestructibleEnv : MonoBehaviour, IHit
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

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
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _Destroy();
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
        Destroy(gameObject);
    }
}
