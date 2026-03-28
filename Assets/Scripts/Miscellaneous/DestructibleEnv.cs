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

    public void OnHitRepel(float damage, float repelForce, Vector2 repelDirection)
    {
        TakeDamage(damage);
    }
    public void OnHitStun(float damage, float stunDuration)
    {
        TakeDamage(damage);
    }
    #endregion IHit Implementation

    private void _Destroy()
    {
        // Add destruction effects here (e.g., particle effects, sound, etc.)
        Destroy(gameObject);
    }
}
