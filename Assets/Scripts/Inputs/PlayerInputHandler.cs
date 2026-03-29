using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerRole Role { get; private set; }

    private PlayerMapActions actions;

    private InputAction moveAction;
    private InputAction meleeAction;
    private InputAction dashAction;

    private InputAction aimAction;
    private InputAction shootAction;
    private InputAction aoeAction;

    public void Initialize(PlayerRole role, Gamepad gamepad)
    {
        Role = role;

        actions = new PlayerMapActions();

        moveAction = actions.Player.Move;
        meleeAction = actions.Player.Melee;
        dashAction = actions.Player.Dash;

        aimAction = actions.Player.AimCursor;
        shootAction = actions.Player.Shoot;
        aoeAction = actions.Player.AOEAttack;

        if (gamepad != null)
        {
            actions.devices = new InputDevice[] { gamepad };
        }
        else
        {
            actions.devices = new InputDevice[] { Keyboard.current };
        }

        actions.Enable();
    }

    private void OnDisable()
    {
        actions?.Disable();
    }

    //////////////////// INPUT LISTENER ////////////////////////

    // MOVEMENT INPUT 
    public Vector2 GetMovement()
    {
        if (Role != PlayerRole.Movement)
            return Vector2.zero;

        return moveAction.ReadValue<Vector2>();
    }

    public bool MeleeReleased()
    {
        return Role == PlayerRole.Movement &&
               meleeAction.WasReleasedThisFrame();
    }
    public bool DashReleased()
    {
        return Role == PlayerRole.Movement &&
               dashAction.WasReleasedThisFrame();
    }

    // SHOOT INPUT
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
    public bool AOEReleased()
    {
        return Role == PlayerRole.Shoot &&
               aoeAction.WasReleasedThisFrame();
    }

    // HOLD
    public bool ShootHold()
    {
        return Role == PlayerRole.Shoot &&
               shootAction.IsPressed();
    }
    public bool MeleeHold()
    {
        return Role == PlayerRole.Movement &&
               meleeAction.IsPressed();
    }
    public bool DashHold()
    {
        return Role == PlayerRole.Movement &&
               dashAction.IsPressed();
    }

    public bool AOEHold()
    {
        return Role == PlayerRole.Shoot &&
               aoeAction.IsPressed();
    }


    //public bool AOEPressed()
    //{
    //    return Role == PlayerRole.Shoot &&
    //           aoeAction.WasPressedThisFrame();
    //}

    //public bool UltimateComboPressed()
    //{
    //    if (Role == PlayerRole.Movement)
    //    {
    //        return dashAction.IsPressed();
    //    }

    //    if (Role == PlayerRole.Shoot)
    //    {
    //        return aoeAction.IsPressed();
    //    }

    //    return false;
    //}
}