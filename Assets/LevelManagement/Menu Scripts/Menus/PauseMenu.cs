using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : Menu
{

    public void OnResumePressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        MenuManager.Instance.ClearMenu();
        MenuManager.Instance.CloseMenu();
    }

    public void OnRestartPressed()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound("UI_Submit");
        LevelLoader.ReloadLevel();
    }
    public void OnSettingsPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        MenuManager.Instance.OpenMenu(MenuManager.Instance.GetSettingsMenu());
    }

    public void OnChangeRolePressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        MenuManager.Instance.OpenMenu(MenuManager.Instance.GetPlayerChoiceMenu());
    }

    public void OnMainMenuPressed()
    {
        MenuManager.Instance.CloseMenu();
        AudioManager.Instance.StopMusic();
        // Détruire le GameManager existant
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
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

