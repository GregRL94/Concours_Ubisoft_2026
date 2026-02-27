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
        prm.DefaultRoleDebug();

        if (prm == null || !prm.AreRolesValid())
        {
            Debug.LogError("Roles invalides !");
            return;
        }

        if (mecha == null)
        {
            Debug.LogError("Mecha manquant !");
            return;
        }

        // Cree les input handler de chaque joueur
        GameObject p1Obj = new GameObject("P1_Input");
        GameObject p2Obj = new GameObject("P2_Input");

        var p1Handler = p1Obj.AddComponent<PlayerInputHandler>();
        var p2Handler = p2Obj.AddComponent<PlayerInputHandler>();

        // Initialise les roles des joueurs 
        p1Handler.Initialize(prm.Player1Role, prm.Player1Gamepad);
        p2Handler.Initialize(prm.Player2Role, prm.Player2Gamepad);

        // Initialise les inputs des joueurs selon leur role choisi vers le mecha
        mecha.GameplayInitialize(p1Handler, p2Handler);

        Debug.Log("Coop Gameplay Initialisé");
    }


}