using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuInputListener : MonoBehaviour
{
    public static event Action<Vector2> UINavigate;

    private PlayerMapActions inputs;
    private MenuManager menuManager;
    private AudioManager audioManager;

    private const string SCENE_MAIN_MENU = "MainMenu";

    [Header("Navigation Menus Settings")]
    [SerializeField] private float firstRepeatDelay = 0.35f;
    [SerializeField] private float repeatRate = 0.12f;

    [Header("Navigation Resolution Settings")]
    [SerializeField] private float navigateCooldown;
    private float lastNavigateTime;

    private Vector2 currentDir;
    private float nextRepeatTime;
    private bool isHolding;

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

        // d-pad menu
        inputs.UI.Navigate.started += OnNavigateStarted;
        // d-pad resolution
        inputs.UI.Navigate.performed += OnNavigate;
        // d-pad menu holding
        inputs.UI.Navigate.canceled += OnNavigateCanceled;

        inputs.UI.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        if (inputs == null)
            return;

        inputs.UI.Cancel.performed -= OnCancel;
        inputs.UI.Submit.performed -= OnSubmit;

        inputs.UI.Navigate.started -= OnNavigateStarted;
        inputs.UI.Navigate.performed -= OnNavigate;
        inputs.UI.Navigate.canceled -= OnNavigateCanceled;

        inputs.UI.Pause.performed -= OnPause;
    }

    private void Update()
    {
        if (!isHolding)
            return;

        if (Time.unscaledTime >= nextRepeatTime)
        {
            UINavigate?.Invoke(currentDir);
            nextRepeatTime = Time.unscaledTime + repeatRate;
        }
    }

    private void OnNavigateStarted(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        if (dir.magnitude < 0.9f)
            return;

        dir = new Vector2(
            Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? Mathf.Sign(dir.x) : 0,
            Mathf.Abs(dir.y) > Mathf.Abs(dir.x) ? Mathf.Sign(dir.y) : 0
        );

        currentDir = dir;
        isHolding = true;


        nextRepeatTime = Time.unscaledTime + firstRepeatDelay;

        UINavigate?.Invoke(currentDir);
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        if (Time.unscaledTime - lastNavigateTime < navigateCooldown)
            return;

        lastNavigateTime = Time.unscaledTime;

        UINavigate?.Invoke(dir);
    }

    private void OnNavigateCanceled(InputAction.CallbackContext context)
    {
        isHolding = false;
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (PlayerChoiceMenu.Instance != null && PlayerChoiceMenu.Instance.HasLockedSelection()) return;

        if (IsMainMenuScene() && menuManager.HasOpenMenu && !menuManager.IsMainMenuActive())
        {
            menuManager.CloseMenu();
            audioManager.PlaySound("UI_Back");
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (menuManager.HasOpenMenu)
        {
            //audioManager.PlaySound("UI_Submit");
        }
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
                return;
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

}