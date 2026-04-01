using Unity.Cinemachine;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private float _damage;
    private LayerMask _explosionHitsWhat;

    void Awake()
    {
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulse();
        }
    }

    public void SetupExplosion(float damage, LayerMask explosionHitsWhat)
    {
        _damage = damage;
        _explosionHitsWhat = explosionHitsWhat;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _explosionHitsWhat) != 0)
        {
            if (collision.TryGetComponent<IHit>(out IHit hit))
            {
                hit.OnHit(_damage);
            }
        }
    }
}
