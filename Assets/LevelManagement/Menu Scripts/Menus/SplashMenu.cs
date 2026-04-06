using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SplashMenu : Menu
{
    [Header("Transition")]
    [SerializeField] private FadeTransition mainMenuTransition;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    [SerializeField] private RectTransform pressAnyButtonText;
    [SerializeField] private Animator animText;

    private bool hasPressed = false;

    void Start()
    {
        AudioManager.Instance.PlayMusic("Music_SplashScreen");
    }

    void Update()
    {
        if (hasPressed) return;

        // Keyboard check
        bool keyboardPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;

        // Gamepad buttons only (ignore joysticks)
        bool gamepadButtonPressed = false;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad == null) continue;

            if (gamepad.allControls
                .OfType<ButtonControl>()
                .Any(b => b.wasPressedThisFrame))
            {
                gamepadButtonPressed = true;
                break;
            }
        }
        // Si clavier ou bouton gamepad pressé
        if (keyboardPressed || gamepadButtonPressed)
        {
            hasPressed = true;
            AudioManager.Instance.PlaySound("SFX_PressAnyButton");
            if (animText) animText.SetTrigger("pressed");
            OnAnyButtonPressed();
        }
    }
    
    private void OnAnyButtonPressed()
    {
        TransitionManager.Instance.SplashToScene(
            mainMenuSceneName,
            mainMenuTransition,
            1f // pause
        );
    }
}