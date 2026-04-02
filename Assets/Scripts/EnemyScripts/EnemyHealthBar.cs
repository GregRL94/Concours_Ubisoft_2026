using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _healthBarCanvas;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _damagedHealthSlider;
    [SerializeField] private float _damagedHealthLerpSpeed = 0.5f;
    private float _maxHealth;
    private float _currentHealth;
    private Quaternion _initialRotation;
    private float _offsetY;
    private Coroutine _damageLerpCoroutine;

    private void Awake()
    {
        _initialRotation = _healthBarCanvas.transform.rotation;
        _offsetY = _healthBarCanvas.transform.position.y - transform.position.y;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeHealthBar();
    }

    private void LateUpdate()
    {
        // Make the health bar face the camera
        _healthBarCanvas.transform.rotation = _initialRotation;
        _healthBarCanvas.transform.position = transform.position + _offsetY * Vector3.up;
    }

    private void InitializeHealthBar()
    {
        if (TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
        {
            _maxHealth = enemyAI.data.maxHealth;
        }
        else if (TryGetComponent<EnemySpawner>(out EnemySpawner enemySpawner))
        {
            _maxHealth = enemySpawner.health;
        }
        _currentHealth = _maxHealth;
        _healthSlider.value = _currentHealth / _maxHealth;
        _damagedHealthSlider.value = _currentHealth / _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        _healthSlider.value = _currentHealth / _maxHealth;
        if (_damageLerpCoroutine != null) { StopCoroutine(_damageLerpCoroutine); }
        _damageLerpCoroutine = StartCoroutine(LerpDamagedHealth(_damagedHealthSlider.value, _currentHealth / _maxHealth));
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
