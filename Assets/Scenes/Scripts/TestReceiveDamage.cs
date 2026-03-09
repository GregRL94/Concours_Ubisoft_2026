using UnityEngine;

public class TestReceiveDamage : MonoBehaviour, IHit
{
    [SerializeField] private float _health = 50f;
    private Rigidbody2D _rb2D;

    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_health <= 0)
        {
            _Destroy();
        }
    }

    public void OnHit(float damage)
    { 
        _health -= damage;
        Debug.Log("Object hit with damage: " + damage + ", remaining health: " + _health);
    }

    public void OnHit(float damage, float repelForce, Vector2 repelDirection)
    {
        _health -= damage;
        _rb2D.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);
    }

    private void _Destroy()
    {
        Debug.Log("Object destroyed");
        Destroy(gameObject);
    }
}
