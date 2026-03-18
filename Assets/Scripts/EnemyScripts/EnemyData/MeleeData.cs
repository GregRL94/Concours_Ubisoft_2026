using UnityEngine;

[CreateAssetMenu(fileName = "MeleeData", menuName = "Enemy/MeleeData")]
public class MeleeData : EnemyData
{
    [Header("Reglages Corps a corps")] 
    public float attackSpeed = 1.5f; //Temps entre deux coups
    public float hitRadius = 1.0f; // Zone de detection du coup
}
