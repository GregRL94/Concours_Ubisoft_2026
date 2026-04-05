using System.Collections.Generic;
using UnityEngine;

public class DestructibleEnv : MonoBehaviour, IHit
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private int _thresholdDamagedSound = 15;
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private GameObject _destroyedEffect;
    [SerializeField] private List<Sprite> _destroyedSprite = new List<Sprite>();
    private float _currentHealth;
    private float _damageCounter = 0f;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void TakeDamage(float damage)
    {
        if (TryGetComponent<FlashEffect>(out var flashEffect)) { flashEffect.Flash(); }
        Collider2D hitCollider = GetComponent<Collider2D>();
        float randX = Random.Range(hitCollider.bounds.min.x, hitCollider.bounds.max.x);
        float randY = Random.Range(hitCollider.bounds.min.y, hitCollider.bounds.max.y);
        Instantiate(_hitEffect, new Vector2(randX, randY), Quaternion.identity);
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
        AudioManager.Instance.PlaySound("SFX_Env_debris_destroy");
        Instantiate(_destroyedEffect, transform.position, Quaternion.identity);
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            int randIndex = Random.Range(0, _destroyedSprite.Count);
            spriteRenderer.sprite = _destroyedSprite[randIndex];
        }
        else
        {
            int randIndex1 = Random.Range(0, 1);
            int randIndex2 = Random.Range(2, 3);
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _destroyedSprite[randIndex1];
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = _destroyedSprite[randIndex2];
        }
        GetComponent<Collider2D>().enabled = false;
    }
}
