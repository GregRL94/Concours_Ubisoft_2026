using UnityEngine;
[CreateAssetMenu(fileName = "NewSniperData", menuName = "Enemy/SniperData")]
public class SniperData: EnemyData
{
    [Header("Reglages de tir")] 
    public float _fireRate = 2f;
    public GameObject _projectilePrefab;
    public GameObject _explosionEffect;
    public float _speed;
    public float _lifetime;
    public LayerMask _impactLayerMask;

    [Header("Parametres de son")]
    public string soundShoot = "SFX_Ennemi_boltbat_tir";
    public string soundDeath = "SFX_Ennemi_boltbat_mort";
}
