using UnityEngine;

public class LaserShot : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffect;
    private float _speed;
    private float _damage;
    private float _lifetime;
    private LayerMask _impactLayerMask;

    private float _timer;

    // Update is called once per frame
    void Update()
    {
        Move();
        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        { 
            _Destroy();
        }
    }

    public void SetupLaserShoot(float speed, float damage, float lifeTime, LayerMask impactLayerMask)
    {
        _speed = speed;
        _damage = damage;
        _lifetime = lifeTime;
        _impactLayerMask = impactLayerMask;
    }

    private void Move()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    private void _Destroy()
    {
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!(((1 << collider.gameObject.layer) & _impactLayerMask) != 0)) { return; }
        if (collider.TryGetComponent(out IHit hitComponent))
        {
            Debug.Log($"Laser hit {collider.gameObject.name} for {_damage} damage.");
            hitComponent.OnHit(_damage);
        }
        _Destroy();
    }
}
