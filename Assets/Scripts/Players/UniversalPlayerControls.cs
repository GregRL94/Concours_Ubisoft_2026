using UnityEngine;
using UnityEngine.InputSystem;

public class UniversalPlayerControls : MonoBehaviour
{

    #region Attributes & Properties
    private InputActionAsset _inputActionAsset;
    private InputActionMap _actionMap;
    private PlayerActions _playerActions;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Input Callbacks
    private void OnDpadActions(InputAction.CallbackContext context)
    {
        DpadInputs = context.ReadValue<Vector2>(); // Handle D-pad inputs
    }

    private void OnMainAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerActions.PerformMainAbility(); // Handle main ability activation
        }
    }

    private void OnSecondaryAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerActions.PerformSecondaryAbility(); // Handle secondary ability activation
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
            dPadActions.performed += OnDpadActions;
            mainAbility.performed += OnMainAbility;
            secondaryAbility.performed += OnSecondaryAbility;
        }
        else
        {
            dPadActions.performed -= OnDpadActions;
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
