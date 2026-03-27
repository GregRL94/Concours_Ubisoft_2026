using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private const string SCENE_MAIN_MENU = "MainMenu";

    private Stack<Menu> menuStack = new Stack<Menu>();

    [Header("Canvas Menus")]
    [SerializeField] private Menu mainMenu;
    [SerializeField] private Menu pauseMenu;
    [SerializeField] private Menu gameOverMenu;
    [SerializeField] private Menu endMenu;
    [SerializeField] private Menu nextLevelMenu;
    [SerializeField] private Menu playerChoiceMenu;
    [SerializeField] private Menu settingsMenu;
    [SerializeField] private Menu accessibilityMenu;

    [Header("Debug Variables")]
    [SerializeField] private GameObject menuParent;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(HandleSceneLoaded(scene));
    }

    private IEnumerator HandleSceneLoaded(Scene scene)
    {
        // attendre 1 frame
        yield return null;

        ClearMenu();

        mainMenu = FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);
        pauseMenu = FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);
        playerChoiceMenu = FindAnyObjectByType<PlayerChoiceMenu>(FindObjectsInactive.Include);
        gameOverMenu = FindAnyObjectByType<GameOverMenu>(FindObjectsInactive.Include);
        endMenu = FindAnyObjectByType<EndMenu>(FindObjectsInactive.Include);
        accessibilityMenu = FindAnyObjectByType<AccessibilityMenu>(FindObjectsInactive.Include);
        nextLevelMenu = FindAnyObjectByType<NextLevelMenu>(FindObjectsInactive.Include);
        settingsMenu = FindAnyObjectByType<SettingsMenu>(FindObjectsInactive.Include);

        // reset visuel
        if (menuParent != null)
            HideMenusDebug();

        if (scene.name == SCENE_MAIN_MENU && mainMenu != null)
        {
            // force activation parent si jamais
            if (!mainMenu.gameObject.activeInHierarchy)
                mainMenu.gameObject.SetActive(true);

            OpenMenu(mainMenu);
        }
    }

    private void HideMenusDebug()
    {
        if (menuParent == null) return;

        for (int i = 0; i < menuParent.transform.childCount; i++)
        {
            menuParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OpenMenu(Menu menu)
    {
        if (menu == null) return;

        if (menuStack.Count > 0)
        {
            Menu top = menuStack.Peek();
            if (top != null)
                top.HideDisplay();
        }

        menuStack.Push(menu);

        // sécurité si désactivé par HideMenusDebug
        if (!menu.gameObject.activeSelf)
            menu.gameObject.SetActive(true);

        menu.ShowDisplay();
    }

    public void CloseMenu()
    {
        if (menuStack.Count == 0)
            return;

        Menu top = menuStack.Pop();
        top.HideDisplay();

        if (menuStack.Count > 0)
            menuStack.Peek().ShowDisplay();
    }

    public void ClearMenu()
    {
        while (menuStack.Count > 0)
        {
            Menu m = menuStack.Pop();
            if (m != null)
                m.HideDisplay();
        }
    }

    // GETTERS / RETURN BOOL
    public bool HasOpenMenu => menuStack.Count > 0;

    public bool IsMainMenuScene() => SceneManager.GetActiveScene().name == SCENE_MAIN_MENU;

    public bool IsMainMenuActive()
    {
        return mainMenu != null && mainMenu.gameObject.activeInHierarchy;
    }

    public bool IsPauseMenuActive()
    {
        return pauseMenu != null && pauseMenu.gameObject.activeInHierarchy;
    }

    public bool IsPlayerChoiceMenuActive()
    {
        return playerChoiceMenu != null && playerChoiceMenu.gameObject.activeInHierarchy;
    }

    public bool IsSettingsMenuActive()
    {
        return settingsMenu != null && settingsMenu.gameObject.activeInHierarchy;
    }

    public PauseMenu GetPauseMenu()
    {
        if (pauseMenu == null)
            pauseMenu = FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);

        return pauseMenu as PauseMenu;
    }

    public MainMenu GetMainMenu()
    {
        if (mainMenu == null)
            mainMenu = FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);

        return mainMenu as MainMenu;
    }

    public PlayerChoiceMenu GetPlayerChoiceMenu()
    {
        if (playerChoiceMenu == null)
            playerChoiceMenu = FindAnyObjectByType<PlayerChoiceMenu>(FindObjectsInactive.Include);

        return playerChoiceMenu as PlayerChoiceMenu;
    }

    public GameOverMenu GetGameOverMenu()
    {
        if (gameOverMenu == null)
            gameOverMenu = FindAnyObjectByType<GameOverMenu>(FindObjectsInactive.Include);

        return gameOverMenu as GameOverMenu;
    }

    public EndMenu GetEndMenu()
    {
        if (endMenu == null)
            endMenu = FindAnyObjectByType<EndMenu>(FindObjectsInactive.Include);

        return endMenu as EndMenu;
    }

    public AccessibilityMenu GetAccessibilityMenu()
    {
        if (accessibilityMenu == null) 
            accessibilityMenu = FindAnyObjectByType<AccessibilityMenu>(FindObjectsInactive.Include);

        return accessibilityMenu as AccessibilityMenu;
    }

    public NextLevelMenu GetNextLevelMenu()
    {
        if (nextLevelMenu == null)
            nextLevelMenu = FindAnyObjectByType<NextLevelMenu>(FindObjectsInactive.Include);

        return nextLevelMenu as NextLevelMenu;
    }

    public SettingsMenu GetSettingsMenu()
    {
        if (settingsMenu == null)
            settingsMenu = FindAnyObjectByType<SettingsMenu>(FindObjectsInactive.Include);

        return settingsMenu as SettingsMenu;
    }
}