using UnityEngine;

[CreateAssetMenu(fileName = "NewKamikazeData", menuName = "Enemy/KamikazeData")]
public class KamikazeData : EnemyData
{
    [Header("Reglages de l'explosion")] 
    public float explosionRadius = 3f;
    public float patrolRadius = 22.5f;
    public float explosionDamage = 50f;
    public GameObject explosionEffect;
    public float explosionDelay = 1f; //Petit temps avant de sauter
}
