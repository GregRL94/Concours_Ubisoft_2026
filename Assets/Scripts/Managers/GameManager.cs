using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FadeTransition fadeTransition;

    private void Update()
    {
        // Vérifie si la touche G est pressée
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        TransitionManager.Instance.TransitionToScene(
            "LevelTest2",
            fadeTransition,
            0.5f
        );
    }
}