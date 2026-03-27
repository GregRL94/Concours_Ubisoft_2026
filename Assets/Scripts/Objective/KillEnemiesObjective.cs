using UnityEngine;

public class KillEnemiesObjective : Objective
{
    [SerializeField] private string objectiveText;
    public override void Begin()
    {
        UIManager.Instance.UpdateObjective(objectiveText);
    }

}