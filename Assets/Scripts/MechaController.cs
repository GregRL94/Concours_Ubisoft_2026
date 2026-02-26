using UnityEngine;

public class MechaController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    private PlayerInputHandler movementPlayer;
    private PlayerInputHandler shootPlayer;


    // Injecte les inputs des joueurs selon leur role choisi 
    public void GameplayInitialize(PlayerInputHandler p1, PlayerInputHandler p2)
    {
        if (p1.Role == PlayerRoleManager.PlayerRole.Movement)
        {
            movementPlayer = p1;
            shootPlayer = p2;
        }
        else
        {
            movementPlayer = p2;
            shootPlayer = p1;
        }
    }

    // Mettre a jour la fonctionnalité des joueurs
    private void Update()
    {
        if (movementPlayer == null || shootPlayer == null)
            return;

        HandleMovement();
        HandleCombat();
    }

    private void HandleMovement()
    {
        Vector2 move = movementPlayer.GetMovement();

        transform.Translate(move * movementSpeed * Time.deltaTime);

        if (movementPlayer.MeleePressed())
            Debug.Log("MELEE");

        if (movementPlayer.GrapplePressed())
            Debug.Log("GRAPPLE");
    }

    private void HandleCombat()
    {
        Vector2 aim = shootPlayer.GetAim();

        if (aim != Vector2.zero)
            Debug.Log($"AIM {aim}");

        if (shootPlayer.ShootPressed())
            Debug.Log("SHOOT");

        if (shootPlayer.AOEPressed())
            Debug.Log("AOE");
    }
}