using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Menu
{
    [Header("Main Menu Options")]
    [SerializeField] private Menu settingsMenu;
    [SerializeField] private Menu creditsMenu;
    [SerializeField] private Menu playerChoiceMenu;

    [Header("Transition & Loading")]
    [SerializeField] private float playDelay = 0.5f;
    //[SerializeField] private float pauseDelay = 2f;
    [SerializeField] private FadeTransition fadeTransition;
    //[SerializeField] private string sceneName = "Level";

    private void OnEnable()
    {
        AudioManager.Instance.PlayMusic("Music_MainMenu");
    }
    public void OnPlayPressed()
    {
        StartCoroutine(OnPlayPressedRoutine());
    }

    private IEnumerator OnPlayPressedRoutine()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        yield return new WaitForSeconds(playDelay);
        //TransitionManager.Instance.TransitionToScene(sceneName, fadeTransition, pauseDelay);
        //TransitionManager.Instance.FadeInCurrentScene(null, MenuManager.Instance.GetPlayerChoiceMenu(), 0f);
        if (playerChoiceMenu != null)
            MenuManager.Instance.OpenMenu(playerChoiceMenu);
    }

    public void OnSettingsPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        if (settingsMenu != null)
            MenuManager.Instance.OpenMenu(settingsMenu);
    }

    public void OnCreditsPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        if (creditsMenu != null)
            MenuManager.Instance.OpenMenu(creditsMenu);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exit option for editor 
#endif
    }

}
