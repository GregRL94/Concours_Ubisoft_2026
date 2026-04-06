using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Menu
{
    [Header("Main Menu Options")]
    [SerializeField] private Menu settingsMenu;
    [SerializeField] private Menu creditsMenu;
    [SerializeField] private Menu playerChoiceMenu;
    [SerializeField] private Menu accessibilityMenu;

    [Header("Menu Transition")]
    [SerializeField] private float playDelay = 0.5f;
    //[SerializeField] private FadeTransition fadeTransition;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float fadeInDuration = 0.3f;

    private void OnEnable()
    {
        if(AudioManager.Instance)
            AudioManager.Instance.PlayMusic("Music_MainMenu");
    }
    public void OnPlayPressed()
    {
        StartCoroutine(OnPlayPressedRoutine());
    }

    private IEnumerator OnPlayPressedRoutine()
    {
        AudioManager.Instance.PlaySound("UI_startgame");

        //TransitionManager.Instance.FadeInCurrentScene(null, MenuManager.Instance.GetPlayerChoiceMenu(), 0f);
        //if (playerChoiceMenu != null)
        //    MenuManager.Instance.OpenMenu(playerChoiceMenu);

        yield return new WaitForSeconds(playDelay);

        TransitionManager.Instance.TransitionBetweenMenus(
            this,
            playerChoiceMenu,
            fadeOutDuration,
            fadeInDuration
        );
    }

    public void OnSettingsPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        if (settingsMenu != null)
            MenuManager.Instance.OpenMenu(settingsMenu);
    }
    public void OnAccessibilityPressed()
    {
        AudioManager.Instance.PlaySound("UI_Submit");
        if (creditsMenu != null)
            MenuManager.Instance.OpenMenu(accessibilityMenu);
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
