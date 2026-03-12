using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Missile : MonoBehaviour
{
    private TrailRenderer _trailRenderer;
    private ParticleSystem _particleSystem;
    private Light2D _light2D;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _lifetime;
    [SerializeField] private float _damage;
    [SerializeField] private float _holdRotationTimer;
    [SerializeField] private float _holdMovementTimer;
    [SerializeField] private LayerMask _impactLayerMask;
    [SerializeField] private GameObject _target;

    
    private float _timer;
    private bool _rotationActive = false;
    private bool _movementActive = false;
    private bool _effectsActive = false;

    private void Start()
    {
        if (TryGetComponent(out TrailRenderer trailRenderer))
        {
            _trailRenderer = trailRenderer;
        }

        _particleSystem = GetComponentInChildren<ParticleSystem>();

        _light2D = GetComponentInChildren<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if (!_rotationActive && _timer > _holdRotationTimer) { _rotationActive = true; }

        if (!_movementActive && _timer > _holdMovementTimer) { _movementActive = true; }

        if (!_effectsActive && _rotationActive && _movementActive)
        {
            ActivateMissileEffects();
        }
        if (_rotationActive)
        {
            RotateTowardsTarget();
        }
        if (_movementActive)
        {
            Move();
        }
        if (_timer >= _lifetime || _target == null)
        {
            _Destroy();
        }
    }

    public void SetupMissile(float speed, float rotationSpeed, float lifetime, float damage)
    {
        _speed = speed;
        _rotationSpeed = rotationSpeed;
        _lifetime = lifetime;
        _damage = damage;
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    private void ActivateMissileEffects()
    {
        _effectsActive = true;
        if (_trailRenderer != null) { _trailRenderer.emitting = true; }
        if (_particleSystem != null) { _particleSystem.Play(); }
        if (_light2D != null) { _light2D.enabled = true; }
    }
    private void RotateTowardsTarget()
    {
        if (_target == null) { return; }
        Vector2 dir = _target.transform.position - transform.position;
        float targetAngle = MathUtils.DirToAngleRad(dir.x, dir.y, -90f);
        Vector3 targetRotation = new Vector3(0f, 0f, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), _rotationSpeed * Time.deltaTime);
        Debug.Log("Rotating towards target" + transform.rotation);
    }

    private void Move()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    private void _Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) { return; }
        if (!(((1 << collider.gameObject.layer) & _impactLayerMask) != 0)) { return; }
        if (collider.TryGetComponent(out IHit hitComponent))
        {
            hitComponent.OnHit(_damage);
        }
        Debug.Log("Missile collided with " + collider.gameObject.name);
        _Destroy();
    }
}
