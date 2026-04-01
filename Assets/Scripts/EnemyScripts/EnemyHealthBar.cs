using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{    
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _damagedHealthSlider;
    [SerializeField] private float _damagedHealthLerpSpeed = 0.5f;
    private float _maxHealth;
    private float _currentHealth;
    private Quaternion _initialRotation;
    private Coroutine _damageLerpCoroutine;

    private void Awake()
    {
        _initialRotation = transform.rotation;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _maxHealth = GetComponentInParent<EnemyAI>().data.maxHealth;
        _currentHealth = _maxHealth;
        _healthSlider.value = _currentHealth;
        _damagedHealthSlider.value = _currentHealth;
    }

    private void LateUpdate()
    {
        // Make the health bar face the camera
        transform.rotation = _initialRotation;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        _healthSlider.value = _currentHealth;
        if (_damageLerpCoroutine != null) { StopCoroutine(_damageLerpCoroutine); }
        _damageLerpCoroutine = StartCoroutine(LerpDamagedHealth(_damagedHealthSlider.value, _currentHealth));
    }

    IEnumerator LerpDamagedHealth(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < _damagedHealthLerpSpeed)
        {
            elapsed += Time.deltaTime;
            _damagedHealthSlider.value = Mathf.Lerp(from, to, elapsed / _damagedHealthLerpSpeed);
            yield return null;
        }
        _damagedHealthSlider.value = to;
        yield break;
    }
}
