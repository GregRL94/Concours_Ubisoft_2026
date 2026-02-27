using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputListener : MonoBehaviour
{
    public static event Action<int, Vector2> UINavigate;
    public static event Action<int> UISubmit;
    public static event Action<int> UISubmitReleased;

    private PlayerMapActions inputs;

    [Header("Navigation Repeat")]
    [SerializeField] private float firstRepeatDelay = 0.35f;
    [SerializeField] private float repeatRate = 0.12f;

    private Vector2 lastDir;
    private float nextRepeatTime;
    private bool isHolding;

    // --------------------------------------------------
    // INIT
    // --------------------------------------------------

    private void Awake()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager not ready");
            return;
        }

        inputs = InputManager.Instance.Inputs;
    }

    private void OnEnable()
    {
        if (inputs == null) return;

        inputs.UI.Enable();

        inputs.UI.Navigate.performed += OnNavigate;
        inputs.UI.Cancel.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (inputs == null) return;

        inputs.UI.Navigate.performed -= OnNavigate;
        inputs.UI.Cancel.performed -= OnCancel;
    }

    // --------------------------------------------------
    // Submit Hold Fix
    // --------------------------------------------------

    private void Update()
    {
        CheckSubmitHold(PlayerRoleManager.Instance?.Player1Gamepad, 0);
        CheckSubmitHold(PlayerRoleManager.Instance?.Player2Gamepad, 1);
    }

    private void CheckSubmitHold(Gamepad gamepad, int playerId)
    {
        if (gamepad == null)
            return;

        if (gamepad.buttonSouth.isPressed)
        {
            UISubmit?.Invoke(playerId);
        }
        else if (gamepad.buttonSouth.wasReleasedThisFrame)
        {
            UISubmitReleased?.Invoke(playerId);
        }
    }

    // --------------------------------------------------
    // NAVIGATION 
    // --------------------------------------------------

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        if (dir.magnitude < 0.9f)
        {
            isHolding = false;
            return;
        }

        dir = dir.normalized;

        int playerId = GetPlayerFromDevice(context.control.device);
        if (playerId == -1)
            return;

        if (!isHolding || dir != lastDir)
        {
            isHolding = true;
            lastDir = dir;

            UINavigate?.Invoke(playerId, dir);

            nextRepeatTime = Time.unscaledTime + firstRepeatDelay;
            return;
        }

        if (Time.unscaledTime >= nextRepeatTime)
        {
            UINavigate?.Invoke(playerId, dir);
            nextRepeatTime = Time.unscaledTime + repeatRate;
        }
    }

    // --------------------------------------------------
    // CANCEL
    // --------------------------------------------------

    private void OnCancel(InputAction.CallbackContext context)
    {
        int playerId = GetPlayerFromDevice(context.control.device);

        if (playerId == -1)
            return;

        if (PlayerChoiceMenu.Instance != null)
        {
            AudioManager.Instance.PlaySound("UI_Back");
            PlayerChoiceMenu.Instance.CancelSelection(playerId);
        }
    }

    // --------------------------------------------------
    // DEVICE  PLAYER MAPPING
    // --------------------------------------------------

    private int GetPlayerFromDevice(InputDevice device)
    {
        if (PlayerRoleManager.Instance == null)
            return -1;

        if (device == PlayerRoleManager.Instance.Player1Gamepad)
            return 0;

        if (device == PlayerRoleManager.Instance.Player2Gamepad)
            return 1;

        return -1;
    }
}