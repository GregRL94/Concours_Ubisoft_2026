using UnityEngine;

[CreateAssetMenu(fileName = "NewKamikazeData", menuName = "Enemy/KamikazeData")]
public class KamikazeData : EnemyData
{
    [Header("Reglages de l'explosion")]
    public LayerMask _explosionImpactsWhat;
    public float explosionRadius = 3f;
    public GameObject explosionEffect;
    public float explosionDelay = 1f; //Petit temps avant de sauter
}
