using UnityEngine;

[CreateAssetMenu(fileName = "MeleeData", menuName = "Enemy/MeleeData")]
public class MeleeData : EnemyData
{
    [Header("Reglages Corps a corps")] 
    public float attackSpeed = 1.5f; //Temps entre deux coups
    public float hitRadius = 1.0f; // Zone de detection du coup
    public float repelForce = 5f; //Force de recul appliquee au joueur
    public float stunDuration = 1f; //Duree de stun appliquee au joueur

    [Header("Parametres de son")]
    public int thresholdDamagedSound = 75;
    public string soundAttack = "SFX_Ennemi_minkai_attack";
    public string soundDamaged = "SFX_Ennemi_minkai_damaged";
    public string soundDeath = "SFX_Ennemi_minkai_dead";
}
