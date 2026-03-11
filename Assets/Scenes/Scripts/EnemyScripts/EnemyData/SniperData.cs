using UnityEngine;
[CreateAssetMenu(fileName = "NewSniperData", menuName = "Enemy/SniperData")]
public class SniperData: EnemyData
{
    [Header("Reglages de tir")] 
    public float projectileSpeed = 20f;
    public float fireRate = 2f;
    public GameObject projectilePrefab;
}
