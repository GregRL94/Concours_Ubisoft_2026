using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuInputListener: MonoBehaviour
{
    public static event Action<Vector2> UINavigate;

    private PlayerMapActions inputs;
    private MenuManager menuManager;
    private AudioManager audioManager;

    private const string SCENE_MAIN_MENU = "MainMenu";
    private const string SCENE_LEVEL = "Level";

    // Specific Customizable Input
    [SerializeField] private float navigateCooldown;
    private float lastNavigateTime;
    private bool stickInUse = false;

    private void Awake()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager not ready");
            return;
        }

        inputs = InputManager.Instance.Inputs;
    }

    private void Start()
    {
        menuManager = MenuManager.Instance;
        audioManager = AudioManager.Instance;
    }

    private void OnEnable()
    {
        if (inputs == null)
            return;

        inputs.UI.Enable();
        inputs.UI.Cancel.performed += OnCancel;
        inputs.UI.Submit.performed += OnSubmit;
        inputs.UI.Navigate.performed += OnNavigate;
        inputs.UI.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        if (inputs == null)
            return;

        inputs.UI.Cancel.performed -= OnCancel;
        inputs.UI.Submit.performed -= OnSubmit;
        inputs.UI.Navigate.performed -= OnNavigate;
        inputs.UI.Pause.performed -= OnPause;
    }



    private void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (IsMainMenuScene() && menuManager.HasOpenMenu && !menuManager.IsMainMenuActive())
        {
            menuManager.CloseMenu();
            audioManager.PlaySound("UI_Back");
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        //if (menuManager.HasOpenMenu)
        //{
        //    audioManager.PlaySound("UI_Submit");
        //}
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (IsMainMenuScene())
            return;

        if (menuManager == null)
        {
            menuManager = MenuManager.Instance;
            if (menuManager == null)
                return; // scène en transition
        }

        if (!menuManager.IsPauseMenuActive())
        {
            var pauseMenu = menuManager.GetPauseMenu();
            if (pauseMenu != null)
                menuManager.OpenMenu(pauseMenu);
        }
        else
        {
            menuManager.CloseMenu();
        }
    }


    private bool IsMainMenuScene() =>
        SceneManager.GetActiveScene().name == SCENE_MAIN_MENU;

    private bool IsGameplayScene() =>
        SceneManager.GetActiveScene().name == SCENE_LEVEL;

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        // DEAD ZONE
        if (Mathf.Abs(dir.x) < 0.9f)
        {
            stickInUse = false;
            return;
        }

        if (stickInUse)
            return;

        if (Time.unscaledTime - lastNavigateTime < navigateCooldown)
            return;

        stickInUse = true;
        lastNavigateTime = Time.unscaledTime;

        UINavigate?.Invoke(dir);
    }


}