using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerRoleManager.PlayerRole Role { get; private set; }

    private PlayerMapActions actions;

    private InputAction moveAction;
    private InputAction meleeAction;
    private InputAction grappleAction;

    private InputAction aimAction;
    private InputAction shootAction;
    private InputAction aoeAction;

    // Relie les actions a travers la manette de chaque joueur && no swap
    public void Initialize(PlayerRoleManager.PlayerRole role, Gamepad gamepad)
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

        // NO SWAP CONTROLLER 
        actions.devices = new InputDevice[] { gamepad };

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
        if (Role != PlayerRoleManager.PlayerRole.Movement)
            return Vector2.zero;

        return moveAction.ReadValue<Vector2>();
    }

    public bool MeleePressed()
    {
        return Role == PlayerRoleManager.PlayerRole.Movement &&
               meleeAction.WasPressedThisFrame();
    }

    public bool GrapplePressed()
    {
        return Role == PlayerRoleManager.PlayerRole.Movement &&
               grappleAction.WasPressedThisFrame();
    }

    // SHOOT ROLE
    public Vector2 GetAim()
    {
        if (Role != PlayerRoleManager.PlayerRole.Shoot)
            return Vector2.zero;

        return aimAction.ReadValue<Vector2>();
    }

    public bool ShootPressed()
    {
        return Role == PlayerRoleManager.PlayerRole.Shoot &&
               shootAction.WasPressedThisFrame();
    }

    public bool AOEPressed()
    {
        return Role == PlayerRoleManager.PlayerRole.Shoot &&
               aoeAction.WasPressedThisFrame();
    }
}