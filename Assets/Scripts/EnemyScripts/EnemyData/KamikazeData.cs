using UnityEngine;

[CreateAssetMenu(fileName = "NewKamikazeData", menuName = "KamikazeData")]
public class KamikazeData : EnemyData
{
    [Header("Reglages de l'explosion")] 
    public float explosionRadius = 3f;
    public float explosionDamage = 50f;
    public GameObject explosionEffect;
    public float explosionDelay = 0.5f; //Petit temps avant de sauter
}
