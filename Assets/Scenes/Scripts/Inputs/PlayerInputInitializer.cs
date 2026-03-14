using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MechaController mecha;

    private void Start()
    {
        InitializeGameplay();
    }

    private void InitializeGameplay()
    {
        var prm = PlayerRoleManager.Instance;

        if (prm == null || mecha == null)
            return;

        GameObject p1Obj = new GameObject("P1_Input");
        var p1Handler = p1Obj.AddComponent<PlayerInputHandler>();

        GameObject p2Obj = new GameObject("P2_Input");
        var p2Handler = p2Obj.AddComponent<PlayerInputHandler>();

        bool hasP1Pad = prm.Player1Gamepad != null;
        bool hasP2Pad = prm.Player2Gamepad != null;

        // 2 manettes
        if (hasP1Pad && hasP2Pad)
        {
            p1Handler.Initialize(prm.Player1Role, prm.Player1Gamepad);
            p2Handler.Initialize(prm.Player2Role, prm.Player2Gamepad);
        }
        // 1 manette
        else if (hasP1Pad)
        {
            p1Handler.Initialize(prm.Player1Role, prm.Player1Gamepad);
            p2Handler.Initialize(prm.Player2Role, null); // Keyboard
        }
        // 0 manette
        else
        {
            p1Handler.Initialize(prm.Player1Role, null); // Keyboard
            p2Handler.Initialize(prm.Player2Role, null); // Keyboard
        }

        mecha.GameplayInitialize(p1Handler, p2Handler);
    }
}