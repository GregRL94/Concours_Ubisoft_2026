using UnityEngine;
[CreateAssetMenu(fileName = "NewSniperData", menuName = "Enemy/SniperData")]
public class SniperData: EnemyData
{
    [Header("Reglages de tir")] 
    public float _fireRate = 2f;
    public GameObject _projectilePrefab;
    public GameObject _explosionEffect;
    public float _speed;
    public float _damage;
    public float _lifetime;
    public LayerMask _impactLayerMask;
}
