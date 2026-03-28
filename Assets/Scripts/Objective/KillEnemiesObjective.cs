using UnityEngine;
using UnityEngine.SceneManagement;

public class KillEnemiesObjective : Objective
{
    [SerializeField] private string objectiveText;
    public override void Begin()
    {
        UIManager.Instance.UpdateObjective(objectiveText);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIManager.Instance.UpdateObjective(objectiveText);
        //GameManager.Instance.UpdateMissionUI();

    }
}