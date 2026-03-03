using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerRole Role { get; private set; }

    private PlayerMapActions actions;

    private InputAction moveAction;
    private InputAction meleeAction;
    private InputAction grappleAction;

    private InputAction aimAction;
    private InputAction shootAction;
    private InputAction aoeAction;

    // Relie les actions a travers la manette de chaque joueur && no swap
    public void Initialize(PlayerRole role, Gamepad gamepad)
    {
        Role = role;

        actions = new PlayerMapActions();

        // Movement map
        moveAction = actions.Player.Move;
        meleeAction = actions.Player.Melee;
        grappleAction = actions.Player.GrapplingHook;

        // Shoot map
        aimAction = actions.Player.AimCursor;
        shootAction = actions.Player.Shoot;
        aoeAction = actions.Player.AOEAttack;

        if (gamepad != null)
        {
            // Bind ce gamepad
            actions.devices = new InputDevice[] { gamepad };
            Debug.Log($"Gamepad assigné à {role}");
        }
        else
        {
            // Bind le clavier
            actions.devices = new InputDevice[] { Keyboard.current };
            Debug.Log($"Keyboard assigné à {role}");
        }

        actions.Enable();
    }

    // Input system manquant - désactivé pour sécurité
    private void OnDisable()
    {
        actions?.Disable();
    }

    //////////////////// INPUT LISTENER ////////////////////////

    // MOVEMENT ROLE
    public Vector2 GetMovement()
    {
        if (Role != PlayerRole.Movement)
            return Vector2.zero;

        //Debug.Log($"Player {gameObject.name} Move Input: {moveAction.ReadValue<Vector2>()}");
        return moveAction.ReadValue<Vector2>();
    }

    public bool MeleePressed()
    {
        return Role == PlayerRole.Movement &&
               meleeAction.WasPressedThisFrame();
    }

    public bool GrapplePressed()
    {
        return Role == PlayerRole.Movement &&
               grappleAction.WasPressedThisFrame();
    }

    // SHOOT ROLE
    public Vector2 GetAim()
    {
        if (Role != PlayerRole.Shoot)
            return Vector2.zero;

        return aimAction.ReadValue<Vector2>();
    }

    public bool ShootPressed()
    {
        return Role == PlayerRole.Shoot &&
               shootAction.IsPressed();
    }

    public bool AOEPressed()
    {
        return Role == PlayerRole.Shoot &&
               aoeAction.WasPressedThisFrame();
    }
}