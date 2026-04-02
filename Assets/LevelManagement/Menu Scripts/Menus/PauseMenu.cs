using UnityEngine;

public class PauseMenu : Menu
{

    public void OnResumePressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");

        MenuManager.Instance.ClearMenu();
        MenuManager.Instance.CloseMenu();

        //FindAnyObjectByType<MechaController>(FindObjectsInactive.Include)?.IgnoreInputsForFrames();
        FindAnyObjectByType<MechaController>(FindObjectsInactive.Include)?.BlockInputs();
    }

    public void OnRestartPressed()
    {
        GameManager.Instance.RestartLevel();
    }

    public void OnSettingsPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        MenuManager.Instance.OpenMenu(MenuManager.Instance.GetSettingsMenu());
    }

    public void OnAccessibilityPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        MenuManager.Instance.OpenMenu(MenuManager.Instance.GetAccessibilityMenu());
    }

    public void OnChangeRolePressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        MenuManager.Instance.OpenMenu(MenuManager.Instance.GetPlayerChoiceMenu());
    }

    public void OnMainMenuPressed()
    {
        MenuManager.Instance.CloseMenu();
        OnReturnToMainMenu();
        LevelLoader.LoadMainMenuLevel();
    }

    public void OnQuitPressed()
    {
        Application.Quit();
        #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false; // Exit option for editor 
        #endif
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }

}

