using UnityEngine;
using UnityEngine.InputSystem;

public class UniversalPlayerControls : MonoBehaviour
{

    #region Attributes & Properties
    private InputActionAsset _inputActionAsset; // The attached PlayerInput component's InputActionAsset
    private InputActionMap _actionMap;
    private PlayerActions _playerActions; // The script that handles input behaviour
    private Gamepad _gamepad;

    public Vector2 DpadInputs { get; private set; }
    #endregion

    #region MonoBehaviour Flow
    private void Awake()
    {
        _inputActionAsset = GetComponent<PlayerInput>().actions;
        _actionMap = _inputActionAsset.FindActionMap("Player");
        _playerActions = GetComponent<PlayerActions>();
    }

    private void Start()
    {
        EnablePlayerInputs(true);
    }
    #endregion

    #region Input Callbacks
    private void OnDPadActions(InputAction.CallbackContext context)
    {
        DpadInputs = context.ReadValue<Vector2>(); // Whenever the DPad is used, update the DpadInputs property with the current input value
    }

    private void OnDPadCanceled(InputAction.CallbackContext context)
    {
        DpadInputs = Vector2.zero; // When the DPad input is canceled (released), reset the DpadInputs to zero
    }

    private void OnMainAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerActions.PerformMainAbility();
        }
    }

    private void OnSecondaryAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerActions.PerformSecondaryAbility();
        }
    }
    #endregion

    #region Enable & Disable
    private void EnablePlayerInputs(bool enable)
    {
        InputAction dPadActions = _actionMap.FindAction("DpadActions");
        InputAction mainAbility = _actionMap.FindAction("MainAbility");
        InputAction secondaryAbility = _actionMap.FindAction("SecondaryAbility");

        if (enable)
        {
            _actionMap.Enable();
            dPadActions.performed += OnDPadActions;
            dPadActions.canceled += OnDPadCanceled;
            mainAbility.performed += OnMainAbility;
            secondaryAbility.performed += OnSecondaryAbility;
        }
        else
        {
            dPadActions.performed -= OnDPadActions;
            dPadActions.canceled -= OnDPadCanceled;
            mainAbility.performed -= OnMainAbility;
            secondaryAbility.performed -= OnSecondaryAbility;
            _actionMap.Disable();
        }
    }

    private void OnEnable()
    {
        EnablePlayerInputs(true);
    }

    private void OnDisable()
    {
        EnablePlayerInputs(false);
    }

    private void OnDestroy()
    {
        EnablePlayerInputs(false);
    }
    #endregion
}
