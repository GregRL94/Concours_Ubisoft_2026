using UnityEngine;

public class DestructibleEnv : MonoBehaviour, IHit
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    #region IHit Implementation
    public void OnHit(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _Destroy();
        }
    }

    public void OnHit(float damage, float repelForce, Vector2 repelDirection)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _Destroy();
        }
    }
    #endregion IHit Implementation

    private void _Destroy()
    {
        // Add destruction effects here (e.g., particle effects, sound, etc.)
        Destroy(gameObject);
    }
}
