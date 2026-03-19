using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public abstract void Begin();

    protected void Complete()
    {
        GameManager.Instance.CompleteObjective();
    }


}