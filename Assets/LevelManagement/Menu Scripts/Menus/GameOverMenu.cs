using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : Menu
{
    public void OnRestartPressed()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound("UI_Submit");
        LevelLoader.ReloadLevel();
    }

    public void OnMainMenuPressed()
    {
        AudioManager.Instance.StopMusic();
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
