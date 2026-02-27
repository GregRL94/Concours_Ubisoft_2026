using UnityEngine;

public class LaserShot : MonoBehaviour
{
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

    public void SetupLaserShoot(float speed, float damage, LayerMask impactLayerMask)
    {
        _speed = speed;
        _damage = damage;
        _impactLayerMask = impactLayerMask;
    }

    private void Move()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    private void _Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Laser collided with " + collision.gameObject.name);
        _Destroy();
    }
}
