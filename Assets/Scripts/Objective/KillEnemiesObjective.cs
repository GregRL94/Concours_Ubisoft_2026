using UnityEngine;

public class KillEnemiesObjective : Objective
{
    public int enemiesToKill = 10;

    int killCount;

    void OnEnable()
    {
        //Enemy.OnEnemyKilled += RegisterKill;
    }

    void OnDisable()
    {
        //Enemy.OnEnemyKilled -= RegisterKill;
    }

    public override void Begin()
    {
        killCount = 0;
        UIManager.Instance.UpdateObjective("Aliens à tuer " + killCount + "/" + enemiesToKill);
    }

    void RegisterKill()
    {
        killCount++;

        UIManager.Instance.UpdateObjective("Aliens à tuer " + killCount + "/" + enemiesToKill);

        if (killCount >= enemiesToKill)
        {
            Complete();
        }
    }
}