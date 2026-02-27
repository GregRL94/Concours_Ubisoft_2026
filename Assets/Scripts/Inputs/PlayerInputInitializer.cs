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

        if (prm == null)
        {
            Debug.LogError("PlayerRoleManager manquant !");
            return;
        }

        // Applique role par default si moins que 2 manettes
        //prm.DefaultRoleDebug();

        if (mecha == null)
        {
            Debug.LogError("Mecha manquant !");
            return;
        }

        // Cree les input handler de chaque joueur
        GameObject p1Obj = new GameObject("P1_Input");
        var p1Handler = p1Obj.AddComponent<PlayerInputHandler>();
        p1Handler.Initialize(prm.Player1Role, prm.Player1Gamepad);

        PlayerInputHandler p2Handler = null;
        // Seulement si une deuxième manette existe
        if (prm.Player2Gamepad != null)
        {
            GameObject p2Obj = new GameObject("P2_Input");
            p2Handler = p2Obj.AddComponent<PlayerInputHandler>();
            p2Handler.Initialize(prm.Player2Role, prm.Player2Gamepad);
        }
        else
        {
            Debug.Log("Mode Debug 1 manette : P2 non initialisé");
        }

        mecha.GameplayInitialize(p1Handler, p2Handler);

        Debug.Log("Coop Gameplay Initialisé");
    }
}