using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Unity.VisualScripting;
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
    //[SerializeField] private Menu gameOverMenu;
    //[SerializeField] private Menu endMenu;
    [SerializeField] private Menu playerChoiceMenu;


    [Header("Debug Variables")]
    [SerializeField] private GameObject menuParent;
    // espace in main menu and string main menu

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
        HideMenusDebug();
        if (mainMenu != null && SceneManager.GetActiveScene().name == SCENE_MAIN_MENU)
            OpenMenu(mainMenu);
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
        ClearMenu();

        if (mainMenu == null)
            //mainMenu = FindAnyObjectByType<MainMenu>(true);
            mainMenu = FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);

        if (pauseMenu == null)
            //pauseMenu = FindObjectOfType<PauseMenu>(true);
            pauseMenu = FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);

        if (playerChoiceMenu == null)
            //endMenu = FindObjectOfType<EndMenu>(true);
            playerChoiceMenu = FindAnyObjectByType<PlayerChoiceMenu>(FindObjectsInactive.Include);

        //if (gameOverMenu == null)
        //    //gameOverMenu = FindObjectOfType<GameOverMenu>(true);
        //    gameOverMenu = FindAnyObjectByType<GameOverMenu>(FindObjectsInactive.Include);

        //if (endMenu == null)
        //    //endMenu = FindObjectOfType<EndMenu>(true);
        //    endMenu = FindAnyObjectByType<EndMenu>(FindObjectsInactive.Include);


        HideMenusDebug();

        if (scene.name == SCENE_MAIN_MENU && mainMenu != null) 
            OpenMenu(mainMenu);


    }

    private void HideMenusDebug()
    {
        GameObject[] children = new GameObject[menuParent.transform.childCount];

        for (int i = 0; i < menuParent.transform.childCount; i++)
        {
            children[i] = menuParent.transform.GetChild(i).gameObject;
            children[i].SetActive(false);
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
        menu.ShowDisplay();
    }


    public void CloseMenu()
    {
        if (menuStack.Count == 0 )
            return;

        Menu top = menuStack.Pop();
        //print("top " + top);
        top.HideDisplay();

        if (menuStack.Count > 0)
            menuStack.Peek().ShowDisplay();
    }

    public void ClearMenu()
    {
        while (menuStack.Count > 0) 
        {
            menuStack.Pop().HideDisplay();
        }
    }

    // GETTERS / RETURN BOOL
    public bool HasOpenMenu => menuStack.Count > 0;
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

    //public bool IsGameOverMenuActive()
    //{
    //    return gameOverMenu != null && gameOverMenu.gameObject.activeInHierarchy;
    //}

    public PauseMenu GetPauseMenu()
    {
        if (pauseMenu == null)
            pauseMenu = FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);

        return pauseMenu as PauseMenu;
    }

    public PlayerChoiceMenu GetPlayerChoiceMenu()
    {
     if (playerChoiceMenu == null)
            playerChoiceMenu = FindAnyObjectByType<PlayerChoiceMenu>(FindObjectsInactive.Include);

        return playerChoiceMenu as PlayerChoiceMenu;
    }
    //public GameOverMenu GetGameOverMenu()
    //{
    //    if (gameOverMenu == null)
    //        gameOverMenu = FindAnyObjectByType<GameOverMenu>(FindObjectsInactive.Include);

    //    return gameOverMenu as GameOverMenu;
    //}
    //public EndMenu GetEndMenu()
    //{
    //    if (endMenu == null)
    //        endMenu = FindAnyObjectByType<EndMenu>(FindObjectsInactive.Include);

    //    return endMenu as EndMenu;
    //}
}

